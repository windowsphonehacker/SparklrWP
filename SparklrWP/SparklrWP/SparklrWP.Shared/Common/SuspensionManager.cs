using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SparklrWP.Common
{
    /// <summary>
    /// SuspensionManager acquisisce lo stato sessione globale per semplificare la gestione del ciclo di vita dei processi
    /// per un'applicazione.  Si noti che lo stato sessione verrà cancellato automaticamente in diverse
    /// condizioni e deve essere utilizzato solo per archiviare le informazioni utili da
    /// mantenere nelle sessioni, ma deve essere ignorato in caso di arresto anomalo dell'applicazione o di
    /// aggiornamento.
    /// </summary>
    internal sealed class SuspensionManager
    {
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
        private static List<Type> _knownTypes = new List<Type>();
        private const string sessionStateFilename = "_sessionState.xml";

        /// <summary>
        /// Fornisce l'accesso allo stato sessione globale per la sessione corrente.  Questo stato viene
        /// serializzato da <see cref="SaveAsync"/> e ripristinato da
        /// <see cref="RestoreAsync"/>, pertanto i valori devono poter essere serializzati da
        /// <see cref="DataContractSerializer"/> e devono essere compressi quanto più possibile.  Le stringhe
        /// e gli altri tipi di dati completi sono consigliati.
        /// </summary>
        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        /// <summary>
        /// Elenco di tipi personalizzati forniti a <see cref="DataContractSerializer"/> durante
        /// la lettura e la scrittura dello stato sessione.  Inizialmente, è possibile aggiungere ulteriori tipi vuoti
        /// per personalizzare il processo di serializzazione.
        /// </summary>
        public static List<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        /// <summary>
        /// Salvare l'oggetto <see cref="SessionState"/> corrente.  Tutte le istanze di <see cref="Frame"/>
        /// registrate con <see cref="RegisterFrame"/> conserveranno lo
        /// stack di navigazione corrente, che a sua volta fornisce all'oggetto <see cref="Page"/> attivo la possibilità
        /// di salvare lo stato.
        /// </summary>
        /// <returns>Attività asincrona che riflette il momento in cui è stato salvato lo stato sessione.</returns>
        public static async Task SaveAsync()
        {
            try
            {
                // Salvare lo stato di navigazione per tutti i frame registrati
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                // Serializzare lo stato sessione in modo sincrono per evitare l'accesso asincrono allo stato
                // condiviso
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                // Richiamare un flusso di output per il file SessionState e scrivere lo stato in modo asincrono
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Ripristina l'oggetto <see cref="SessionState"/> precedentemente salvato.  Tutte le istanze di <see cref="Frame"/>
        /// registrate con <see cref="RegisterFrame"/> ripristineranno anche lo stato di navigazione
        /// precedente, che a sua volta fornisce all'oggetto <see cref="Page"/> attivo la possibilità di ripristinare il relativo
        /// stato.
        /// </summary>
        /// <param name="sessionBaseKey">Chiave facoltativa che identifica il tipo di sessione.
        /// Può essere utilizzata per distinguere i diversi scenari di avvio dell'applicazione.</param>
        /// <returns>Attività asincrona che riflette quando lo stato sessione è stato letto.  Non
        /// basarsi sul contenuto di <see cref="SessionState"/> finché questa attività
        /// non viene completata.</returns>
        public static async Task RestoreAsync(String sessionBaseKey = null)
        {
            _sessionState = new Dictionary<String, Object>();

            try
            {
                // Richiamare il flusso di input per il file SessionState
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // Deserializzare lo stato sessione
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                    _sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                // Ripristinare tutti i frame registrati al relativo stato salvato
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame) && (string)frame.GetValue(FrameSessionBaseKeyProperty) == sessionBaseKey)
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        private static DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionBaseKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionBaseKeyParams", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
        private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

        /// <summary>
        /// Registra un'istanza di <see cref="Frame"/> per consentire il salvataggio e il ripristino della
        /// cronologia di navigazione da <see cref="SessionState"/>.  I frame devono essere registrati una volta
        /// subito dopo la creazione se partecipano alla gestione dello stato sessione.  Alla
        /// registrazione, se lo stato è già stato ripristinato per la chiave specificata,
        /// verrà subito ripristinata la cronologia di navigazione.  I successivi richiami di
        /// <see cref="RestoreAsync"/> ripristineranno anche la cronologia di navigazione.
        /// </summary>
        /// <param name="frame">Istanza la cui cronologia di navigazione deve essere gestita da
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">Chiave univoca in <see cref="SessionState"/> utilizzata per
        /// archiviare le informazioni correlate alla navigazione.</param>
        /// <param name="sessionBaseKey">Chiave facoltativa che identifica il tipo di sessione.
        /// Può essere utilizzata per distinguere i diversi scenari di avvio dell'applicazione.</param>
        public static void RegisterFrame(Frame frame, String sessionStateKey, String sessionBaseKey = null)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
            }

            if (!string.IsNullOrEmpty(sessionBaseKey))
            {
                frame.SetValue(FrameSessionBaseKeyProperty, sessionBaseKey);
                sessionStateKey = sessionBaseKey + "_" + sessionStateKey;
            }

            // Utilizzare una proprietà di dipendenza per associare la chiave di sessione a un frame e conservare un elenco di frame di cui
            // deve essere gestito lo stato di navigazione
            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<Frame>(frame));

            // Verificare se lo stato di navigazione può essere ripristinato
            RestoreFrameNavigationState(frame);
        }

        /// <summary>
        /// Rimuove l'associazione a un oggetto <see cref="Frame"/> precedentemente registrato da <see cref="RegisterFrame"/>
        /// da <see cref="SessionState"/>.  Qualsiasi stato di navigazione precedentemente acquisito verrà
        /// rimosso.
        /// </summary>
        /// <param name="frame">Istanza di cui non deve più essere gestita la cronologia di
        /// navigazione.</param>
        public static void UnregisterFrame(Frame frame)
        {
            // Rimuovere lo stato sessione e rimuovere il frame dall'elenco di frame di cui verrà salvato lo stato
            // di navigazione (insieme ai riferimenti deboli che non sono più raggiungibili)
            SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((weakFrameReference) =>
            {
                Frame testFrame;
                return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        /// <summary>
        /// Fornisce l'archiviazione per lo stato sessione associato all'oggetto <see cref="Frame"/> specificato.
        /// Per i frame precedentemente registrati con <see cref="RegisterFrame"/>, lo
        /// stato sessione viene salvato e ripristinato automaticamente come parte dell'oggetto
        /// <see cref="SessionState"/> globale.  I frame non registrati hanno uno stato di passaggio
        /// che può tuttavia essere utile quando vengono ripristinate le pagine eliminate dalla
        /// cache di navigazione.
        /// </summary>
        /// <remarks>Le applicazioni possono scegliere di basarsi su <see cref="NavigationHelper"/> per gestire
        /// lo stato specifico della pagina anziché gestire direttamente lo stato sessione del frame.</remarks>
        /// <param name="frame">Istanza per cui si desidera lo stato sessione.</param>
        /// <returns>Raccolta di stati soggetti allo stesso meccanismo di serializzazione di
        /// <see cref="SessionState"/>.</returns>
        public static Dictionary<String, Object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    // I frame registrati riflettono lo stato sessione corrispondente
                    if (!_sessionState.ContainsKey(frameSessionKey))
                    {
                        _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                    }
                    frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
                }
                else
                {
                    // I frame non registrati hanno uno stato di passaggio
                    frameState = new Dictionary<String, Object>();
                }
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey("Navigation"))
            {
                frame.SetNavigationState((String)frameState["Navigation"]);
            }
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }
    public class SuspensionManagerException : Exception
    {
        public SuspensionManagerException()
        {
        }

        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {

        }
    }
}
