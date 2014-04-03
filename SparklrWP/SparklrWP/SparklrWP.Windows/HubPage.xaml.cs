using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SparklrWP.Data;
using SparklrWP.Common;

// Il modello di progetto applicazione hub universale è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=391955

namespace SparklrWP
{
    /// <summary>
    /// Pagina in cui viene visualizzata una raccolta raggruppata di elementi.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Ottiene l'elemento NavigationHelper utilizzato per favorire la navigazione e la gestione del ciclo di vita dei processi.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ottiene l'elemento DefaultViewModel. È possibile sostituirlo con un modello di visualizzazione fortemente tipizzato.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Popola la pagina con il contenuto passato durante la navigazione.  Vengono inoltre forniti eventuali stati
        /// salvati durante la ricreazione di una pagina in una sessione precedente.
        /// </summary>
        /// <param name="sender">
        /// Origine dell'evento. In genere <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Dati evento che forniscono il parametro di navigazione passato a
        /// <see cref="Frame.Navigate(Type, object)"/> quando la pagina è stata inizialmente richiesta e
        /// un dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Lo stato è null la prima volta che viene visitata una pagina.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: creare un modello dati appropriato per il dominio problematico per sostituire i dati di esempio
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
        }

        /// <summary>
        /// Richiamato in seguito al clic sull'intestazione HubSection.
        /// </summary>
        /// <param name="sender">Hub che contiene l'elemento HubSection sulla cui intestazione è stato fatto clic.</param>
        /// <param name="e">Dati dell'evento in cui è descritto in che modo è stato avviato il clic.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Richiamato in seguito al clic su un elemento all'interno di una sezione.
        /// </summary>
        /// <param name="sender">GridView o ListView
        /// che visualizza l'elemento su cui è stato fatto clic.</param>
        /// <param name="e">Dati dell'evento in cui è descritto l'elemento su cui è stato fatto clic.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Passa alla pagina di destinazione appropriata, configurando la nuova pagina
            // mediante il passaggio delle informazioni richieste come parametro di navigazione
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region Registrazione di NavigationHelper

        /// <summary>
        /// I metodi forniti in questa sezione vengono utilizzati per consentire a
        /// NavigationHelper di rispondere ai metodi di navigazione della pagina.
        /// La logica specifica della pagina deve essere inserita nel gestore eventi per  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// e <see cref="Common.NavigationHelper.SaveState"/>.
        /// Il parametro di navigazione è disponibile nel metodo LoadState 
        /// oltre allo stato della pagina conservato durante una sessione precedente.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
