using SparklrWP.Data;
using SparklrWP.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SparklrWP
{
    /// <summary>
    /// Pagina in cui viene visualizzata una panoramica per un singolo gruppo, compresa un'anteprima degli elementi
    /// all'interno del gruppo.
    /// </summary>
    public sealed partial class SectionPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public SectionPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

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

        /// <summary>
        /// Popola la pagina con il contenuto passato durante la navigazione.  Vengono inoltre forniti eventuali stati
        /// salvati durante la nuova creazione di una pagina in una sessione precedente.
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
            var group = await SampleDataSource.GetGroupAsync((string)e.NavigationParameter);
            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Items;
        }

        /// <summary>
        /// Richiamato in seguito al clic su un elemento.
        /// </summary>
        /// <param name="sender"> GridView in cui viene visualizzato l'elemento su cui è stato fatto clic.</param>
        /// <param name="e">Dati dell'evento in cui è descritto l'elemento su cui è stato fatto clic.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
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