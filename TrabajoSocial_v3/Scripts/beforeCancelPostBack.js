



function onEvaluateExclude()
{

    debugger;
    var resp = "";

    $.ajaxSetup({ async: false });

    $.when(onExludePatient()).done(function (response)
    {
        debugger;

        resp = JSON.parse(response.d);
        if (resp.DetalleError === "not excluded") {
            resp = false;
        }
        else if (resp.DetalleError === "excluded") {
            resp = true;
        }

        $.ajaxSetup({ async: true });
    });   

    return resp;
}

