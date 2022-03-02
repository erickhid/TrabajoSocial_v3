




window.onload = function () {

    debugger;
 
    Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(cancelPostBack);


    function cancelPostBack(sender, args) {
        if (Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
            alert('One postback at a time please');
            args.set_cancel(true);
        }
    }  



}


//function ApplicationLoadHandler(sender, args) {
//    debugger;
//    Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(BeginRequest);

//}

//function BeginRequest(sender, args) {
//    var prm = Sys.WebForms.PageRequestManager.getInstance();
//    var prm2 = Sys.WebForms.PageRequestManager.getInstance();


//    if (prm.get_isInAsyncPostBack() || prm2._postBackSettings.async) {

//        if (args.get_postBackElement().id == 'ctl00_ContentPlaceHolder1_Button1') {

//            prm.abortPostBack();
//            args.set_cancel(true);
//        }
//    }

//}