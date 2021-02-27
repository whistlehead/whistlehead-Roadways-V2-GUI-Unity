using System.Threading;
using System.Windows.Forms;

namespace whistlehead_Roadways_V2_GUI
{
    public partial class LoadingForm : Form
    {
        //Delegate for cross thread call to close
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static LoadingForm loadingForm;

        public LoadingForm()
        {
            InitializeComponent();
            ControlBox = false;
        }

        static public void ShowLoadingScreen()
        {
            // Make sure it is only launched once.    
            if (loadingForm != null) return;
            loadingForm = new LoadingForm();
            Thread thread = new Thread(new ThreadStart(LoadingForm.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static private void ShowForm()
        {
            if (loadingForm != null) Application.Run(loadingForm);
        }

        static public void CloseForm()
        {
            loadingForm?.Invoke(new CloseDelegate(LoadingForm.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            if (loadingForm != null)
            {
                loadingForm.Close();
                loadingForm = null;
            }
        }
    }
}
