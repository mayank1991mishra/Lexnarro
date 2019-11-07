namespace Lexnarro.HelperClasses
{
    public enum ToastType
    {
        error,
        info,
        success,
        warning
    }


    public class ToasterMessage
    {
        public static string Message(ToastType type, string message)
        {
            string msg = @"<script>Command: toastr['" + type + "']('" + message + "');" +
                                            "toastr.options = {" +
                                            "'closeButton': false," +
                                              "'debug': false," +
                                              "'newestOnTop': false," +
                                              "'progressBar': false," +
                                              "'positionClass': 'toast-top-right'," +
                                              "'preventDuplicates': false," +
                                              "'onclick': null," +
                                              "'showDuration': '5000'," +
                                              "'hideDuration': '1000'," +
                                              "'timeOut': '3000'," +
                                              "'extendedTimeOut': '1000'," +
                                              "'showEasing': 'swing'," +
                                              "'hideEasing': 'linear'," +
                                              "'showMethod': 'fadeIn'," +
                                              "'hideMethod': 'fadeOut'" +
                                            "}</script>";
            return msg;
        }

        public static string MessageCenter(ToastType type, string message)
        {
            string msg = @"<script>Command: toastr['" + type + "']('" + message + "');" +
                                            "toastr.options = {" +
                                            "'closeButton': false," +
                                              "'debug': false," +
                                              "'newestOnTop': false," +
                                              "'progressBar': false," +
                                              "'positionClass': 'toast-top-center'," +
                                              "'preventDuplicates': false," +
                                              "'onclick': null," +
                                              "'showDuration': '5000'," +
                                              "'hideDuration': '1000'," +
                                              "'timeOut': '3000'," +
                                              "'extendedTimeOut': '1000'," +
                                              "'showEasing': 'swing'," +
                                              "'hideEasing': 'linear'," +
                                              "'showMethod': 'fadeIn'," +
                                              "'hideMethod': 'fadeOut'" +
                                            "}</script>";
            return msg;
        }
    }
}