namespace UserInterfaceLogic
{
    public interface IUserInterfaceRealization
    {
        public string getFilePathByDialog();
        public void showErrorDialog(Exception exception);

    }
}
