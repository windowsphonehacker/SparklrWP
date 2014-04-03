using SparklrWP.Common;
using SparklrWP.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di progetto applicazione hub universale è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=391955

namespace SparklrWP
{
    /// <summary>
    /// Pagina in cui vengono visualizzati i dettagli per un singolo elemento all'interno di un gruppo.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        } 

        /// <summary>
        /// Ottiene l'elemento <see cref="NavigationHelper"/> associato a questa <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ottiene il modello di visualizzazione per questa <see cref="Page"/>. È possibile sostituirlo con un modello di visualizzazione fortemente tipizzato.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            var item = await SampleDataSource.GetItemAsync((string)e.NavigationParameter);
            this.DefaultViewModel["Item"] = item;
        }

        /// <summary>
        /// Mantiene lo stato associato a questa pagina in caso di sospensione dell'applicazione o se la
        /// viene scartata dalla cache di navigazione.  I valori devono essere conformi ai requisiti di
        /// serializzazione di <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Origine dell'evento. In genere <see cref="NavigationHelper"/></param>
        /// <param name="e">Dati di evento che forniscono un dizionario vuoto da popolare con
        /// uno stato serializzabile.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: salvare qui lo stato univoco della pagina.
        }

        #region Registrazione di NavigationHelper

        /// <summary>
        /// I metodi forniti in questa sezione vengono utilizzati per consentire a
        /// NavigationHelper di rispondere ai metodi di navigazione della pagina.
        /// <para>
        /// La logica specifica della pagina deve essere inserita nei gestori eventi per
        /// <see cref="NavigationHelper.LoadState"/>
        /// e <see cref="NavigationHelper.SaveState"/>.
        /// Il parametro di navigazione è disponibile nel metodo LoadState
        /// oltre allo stato della pagina conservato durante una sessione precedente.
        /// </para>
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.</param>
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