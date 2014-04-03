using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using SparklrWP.Common;

// Il modello di progetto applicazione hub universale è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=391955

namespace SparklrWP
{
    /// <summary>
    /// Fornisci un comportamento specifico dell'applicazione in supplemento alla classe Application predefinita.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Inizializza l'istanza singleton della classe <see cref="App"/>. Si tratta della prima riga del codice creato
        /// eseguita e, come tale, corrisponde all'equivalente logico di main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente.  All'avvio dell'applicazione
        /// verranno utilizzati altri punti di ingresso per aprire un file specifico, per visualizzare
        /// risultati di ricerche e così via.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e il processo di avvio.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Non ripetere l'inizializzazione dell'applicazione se la finestra già dispone di contenuto,
            // assicurarsi solo che la finestra sia attiva
            if (rootFrame == null)
            {
                // Creare un frame che agisca da contesto di navigazione e passare alla prima pagina
                rootFrame = new Frame();

                //Associare il frame a una chiave SuspensionManager                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: modificare questo valore su una dimensione di cache appropriata per l'applicazione
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Ripristinare lo stato della sessione salvata solo se appropriato
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Errore durante il ripristino dello stato.
                        // Si presuppone che non esista alcuno stato e continua
                    }
                }

                // Posizionare il frame nella finestra corrente
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Rimuove l'avvio della navigazione turnstile.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // Quando lo stack di navigazione non viene ripristinato, esegui la navigazione alla prima pagina,
                // configurando la nuova pagina per passare le informazioni richieste come parametro di
                // navigazione
                if (!rootFrame.Navigate(typeof(HubPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Assicurarsi che la finestra corrente sia attiva
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Ripristina le transizioni del contenuto dopo l'avvio dell'applicazione.
        /// </summary>
        /// <param name="sender">Oggetto a cui è associato il gestore.</param>
        /// <param name="e">Dettagli sull'evento di navigazione.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa.  Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}