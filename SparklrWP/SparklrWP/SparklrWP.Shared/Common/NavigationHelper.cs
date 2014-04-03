using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SparklrWP.Common
{
    /// <summary>
    /// NavigationHelper favorisce la navigazione tra le pagine.  Fornisce i comandi utilizzati per 
    /// spostarsi avanti e indietro nonché esegue la registrazione delle scelte rapide standard da tastiera e con il mouse 
    /// shortcuts used to go back and forward in Windows and the hardware back button in
    /// Windows Phone.  In addition it integrates SuspensionManger to handle process lifetime
    /// management and state management when navigating between pages.
    /// </summary>
    /// <example>
    /// Per utilizzare NavigationHelper, seguire i due passaggi seguenti o
    /// iniziare con un elemento BasicPage o un altro modello di elemento per la pagina diverso da BlankPage.
    /// 
    /// 1) Creare un'istanza di NavigationHelper in qualsiasi punto, ad esempio nel 
    ///     costruttore della pagina e registrare un callback per gli eventi LoadState e 
    ///     SaveState.
    /// <code>
    ///     public MyPage()
    ///     {
    ///         this.InitializeComponent();
    ///         var navigationHelper = new NavigationHelper(this);
    ///         this.navigationHelper.LoadState += navigationHelper_LoadState;
    ///         this.navigationHelper.SaveState += navigationHelper_SaveState;
    ///     }
    ///     
    ///     private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    ///     { }
    ///     private async void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
    ///     { }
    /// </code>
    /// 
    /// 2) Registrare la pagina per chiamare un elemento di NavigationHelper ogni volta che la pagina partecipa 
    ///     alla navigazione eseguendo l'override degli eventi <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo"/> 
    ///     e <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom"/>.
    /// <code>
    ///     protected override void OnNavigatedTo(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedTo(e);
    ///     }
    ///     
    ///     protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedFrom(e);
    ///     }
    /// </code>
    /// </example>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame { get { return this.Page.Frame; } }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="NavigationHelper"/>.
        /// </summary>
        /// <param name="page">Riferimento alla pagina corrente utilizzata per la navigazione.  
        /// Questo riferimento consente la manipolazione dei frame e di verificare che 
        /// le richieste di navigazione tramite tastiera si verifichino solo quando la pagina occupa l'intera finestra.</param>
        public NavigationHelper(Page page)
        {
            this.Page = page;

            // Quando questa pagina è parte della struttura ad albero visuale, effettua due modifiche:
            // 1) Esegui il mapping dello stato di visualizzazione dell'applicazione allo stato di visualizzazione per la pagina
            // 2) Handle hardware navigation requests
            this.Page.Loaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#else
                // La navigazione mediante tastiera e mouse è applicabile solo quando la finestra viene occupata per intero
                if (this.Page.ActualHeight == Window.Current.Bounds.Height &&
                    this.Page.ActualWidth == Window.Current.Bounds.Width)
                {
                    // Ascolta la finestra direttamente, in modo che non ne sia richiesto lo stato attivo
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
#endif
            };

            // Annulla le stesse modifiche quando la pagina non è più visibile
            this.Page.Unloaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#else
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
#endif
            };
        }

        #region Supporto per la navigazione

        RelayCommand _goBackCommand;
        RelayCommand _goForwardCommand;

        /// <summary>
        /// <see cref="RelayCommand"/> utilizzato per l'associazione alla proprietà Comando del pulsante Indietro
        /// per spostarsi all'elemento più recente nella cronologia di navigazione all'indietro, se un frame
        /// gestisce la propria cronologia di navigazione.
        /// 
        /// <see cref="RelayCommand"/> viene configurato per utilizzare il metodo virtuale <see cref="GoBack"/>
        /// come l'azione Esegui e <see cref="CanGoBack"/> per CanExecute.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }
                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }
        /// <summary>
        /// <see cref="RelayCommand"/> utilizzato per spostarsi all'elemento più recente nella 
        /// cronologia di navigazione in avanti, se un frame gestisce la propria cronologia di navigazione.
        /// 
        /// <see cref="RelayCommand"/> viene configurato per utilizzare il metodo virtuale <see cref="GoForward"/>
        /// come l'azione Esegui e <see cref="CanGoForward"/> per CanExecute.
        /// </summary>
        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }
                return _goForwardCommand;
            }
        }

        /// <summary>
        /// Metodo virtuale utilizzato dalla proprietà <see cref="GoBackCommand"/>
        /// per determinare se <see cref="Frame"/> può spostarsi all'indietro.
        /// </summary>
        /// <returns>
        /// true se <see cref="Frame"/> contiene almeno una voce 
        /// nella cronologia di navigazione all'indietro.
        /// </returns>
        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }
        /// <summary>
        /// Metodo virtuale utilizzato dalla proprietà <see cref="GoForwardCommand"/>
        /// per determinare se <see cref="Frame"/> può spostarsi in avanti.
        /// </summary>
        /// <returns>
        /// true se <see cref="Frame"/> contiene almeno una voce 
        /// nella cronologia di navigazione in avanti.
        /// </returns>
        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        /// <summary>
        /// Metodo virtuale utilizzato dalla proprietà <see cref="GoBackCommand"/>
        /// per richiamare il metodo <see cref="Windows.UI.Xaml.Controls.Frame.GoBack"/>.
        /// </summary>
        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
        /// <summary>
        /// Metodo virtuale utilizzato dalla proprietà <see cref="GoForwardCommand"/>
        /// per richiamare il metodo <see cref="Windows.UI.Xaml.Controls.Frame.GoForward"/>.
        /// </summary>
        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Invoked when the hardware back button is pressed. For Windows Phone only.
        /// </summary>
        /// <param name="sender">Istanza che ha generato l'evento.</param>
        /// <param name="e">Dati evento che descrivono le condizioni che hanno determinato l'evento.</param>
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                this.GoBackCommand.Execute(null);
            }
        }
#else
        /// <summary>
        /// Richiamato per ciascuna sequenza di tasti, compresi i tasti di sistema quali combinazioni con il tasto ALT, quando
        /// questa pagina è attiva e occupa l'intera finestra.  Utilizzato per il rilevamento della navigazione da tastiera
        /// tra pagine, anche quando la pagina stessa no dispone dello stato attivo.
        /// </summary>
        /// <param name="sender">Istanza che ha generato l'evento.</param>
        /// <param name="e">Dati evento che descrivono le condizioni che hanno determinato l'evento.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Esegui ulteriori controlli solo se vengono premuti i tasti Freccia SINISTRA, Freccia DESTRA o i tasti dedicati Precedente
            // o successivo
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // Quando viene premuto il tasto Precedente o ALT+Freccia SINISTRA, torna indietro
                    e.Handled = true;
                    this.GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // Quando viene premuto il tasto Successivo o ALT+Freccia DESTRA, vai avanti
                    e.Handled = true;
                    this.GoForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Richiamato per ciascun clic del mouse, tocco del touch screen o interazione equivalente quando la
        /// pagina è attiva e occupa per intero la finestra.  Utilizzato per il rilevamento del clic del mouse sui pulsanti di tipo browser
        /// Precedente e Successivo per navigare tra pagine.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignora combinazioni di pulsanti con i pulsanti sinistro destro e centrale
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // Se viene premuto Precedente o Successivo (ma non entrambi) naviga come appropriato
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.GoBackCommand.Execute(null);
                if (forwardPressed) this.GoForwardCommand.Execute(null);
            }
        }
#endif

        #endregion

        #region Gestione del ciclo di vita dei processi

        private String _pageKey;

        /// <summary>
        /// Registrare questo evento nella pagina corrente per popolare la pagina
        /// con il contenuto passato durante la navigazione nonché tutti gli stati salvati
        /// forniti durante la ricreazione di una pagina in una sessione precedente.
        /// </summary>
        public event LoadStateEventHandler LoadState;
        /// <summary>
        /// Registrare questo evento nella pagina corrente per conservare
        /// lo stato associato alla pagina corrente nel caso in cui
        /// l'applicazione venga sospesa o la pagina venga eliminata dalla
        /// cache di navigazione.
        /// </summary>
        public event SaveStateEventHandler SaveState;

        /// <summary>
        /// Richiamato quando la pagina sta per essere visualizzata in un Frame.  
        /// Questo metodo chiama <see cref="LoadState"/>, in cui inserire la
        /// logica di gestione del ciclo di vita dei processi e di navigazione specifica della pagina.
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.  La proprietà
        /// Parameter fornisce il gruppo da visualizzare.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Cancella lo stato esistente per la navigazione in avanti quando si aggiunge una nuova pagina allo
                // stack di navigazione
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Passa il parametro di navigazione alla nuova pagina
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                // Passa il parametro di navigazione e lo stato della pagina mantenuto, utilizzando
                // la stessa strategia per caricare lo stato sospeso e ricreare le pagine scartate
                // dalla cache
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]));
                }
            }
        }

        /// <summary>
        /// Richiamato quando questa pagina non verrà più visualizzata in un frame.
        /// Questo metodo chiama <see cref="SaveState"/>, in cui inserire la
        /// logica di gestione del ciclo di vita dei processi e di navigazione specifica della pagina.
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.  La proprietà
        /// Parameter fornisce il gruppo da visualizzare.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            if (this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// Rappresenta il metodo che gestirà l'evento <see cref="NavigationHelper.LoadState"/>
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Rappresenta il metodo che gestirà l'evento <see cref="NavigationHelper.SaveState"/>
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Classe utilizzata per memorizzare i dati degli eventi necessari quando una pagina tenta di caricare uno stato.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// Valore del parametro passato a <see cref="Frame.Navigate(Type, Object)"/> 
        /// quando la pagina è stata inizialmente richiesta.
        /// </summary>
        public Object NavigationParameter { get; private set; }
        /// <summary>
        /// Dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Il valore è null la prima volta che viene visitata una pagina.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="LoadStateEventArgs"/>.
        /// </summary>
        /// <param name="navigationParameter">
        /// Valore del parametro passato a <see cref="Frame.Navigate(Type, Object)"/> 
        /// quando la pagina è stata inizialmente richiesta.
        /// </param>
        /// <param name="pageState">
        /// Dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Il valore è null la prima volta che viene visitata una pagina.
        /// </param>
        public LoadStateEventArgs(Object navigationParameter, Dictionary<string, Object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }
    /// <summary>
    /// Classe utilizzata per memorizzare i dati degli eventi necessari quando una pagina tenta di salvare uno stato.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Dizionario vuoto da popolare con uno stato serializzabile.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="SaveStateEventArgs"/>.
        /// </summary>
        /// <param name="pageState">Dizionario vuoto da popolare con uno stato serializzabile.</param>
        public SaveStateEventArgs(Dictionary<string, Object> pageState)
            : base()
        {
            this.PageState = pageState;
        }
    }
}
