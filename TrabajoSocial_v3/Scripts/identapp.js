




var test = null;

var state = document.getElementById('content-capture');

var myVal = ""; // Drop down selected value of reader 
var reader = "";
var disabled = true;
var startEnroll = false;
var currentFormat = Fingerprint.SampleFormat.Intermediate;
var dataToSend = "";
var begin = new Boolean(true);
var count = "0";
var counterReaders = "";
var ObjstepRecord = new Object();
var actualStep = "";

var deviceTechn = {
    0: "Unknown",
    1: "Optical",
    2: "Capacitive",
    3: "Thermal",
    4: "Pressure"
}

var deviceModality = {
    0: "Unknown",
    1: "Swipe",
    2: "Area",
    3: "AreaMultifinger"
}

var deviceUidType = {
    0: "Persistent",
    1: "Volatile"
}


var FingerprintSdkTest = (function () {

    function FingerprintSdkTest() {
        var _instance = this;
        this.operationToRestart = null;
        this.acquisitionStarted = false;
        this.sdk = new Fingerprint.WebApi;


        this.sdk.onDeviceConnected = function (e) {
            debugger;
            // Detects if the deveice is connected for which acquisition started
            showMessage("Scan your finger");
        };
        this.sdk.onDeviceDisconnected = function (e) {
            // Detects if device gets disconnected - provides deviceUid of disconnected device
            showMessage("Device disconnected");
        };
        this.sdk.onCommunicationFailed = function (e) {
            // Detects if there is a failure in communicating with U.R.U web SDK
            showMessage("Communinication Failed")
        };
        this.sdk.onSamplesAcquired = function (s) {
            debugger;
            // Sample acquired event triggers this function
            sampleAcquired(s);
        };
        this.sdk.onQualityReported = function (e) {
            // Quality of sample aquired - Function triggered on every sample acquired
            document.getElementById("qualityInputBox").value = Fingerprint.QualityCode[(e.quality)];
        }

    }

    FingerprintSdkTest.prototype.startCapture = function ()
    {
        debugger;
        if (this.acquisitionStarted) // Monitoring if already started capturing
            return;
        var _instance = this;
        showMessage("");
        this.operationToRestart = this.startCapture;

        this.sdk.startAcquisition(currentFormat, myVal).then(function () {
            debugger;
            _instance.acquisitionStarted = true;

            //Disabling start once started
            //disableEnableStartStop();

        }, function (error) {
            showMessage(error.message);
        });
    };

    FingerprintSdkTest.prototype.startingCapture = function () {
        //debugger;

        var _instance = this;


        this.operationToRestart = this.startCapture;

        this.sdk.startAcquisition(currentFormat, myVal).then(function () {
            debugger;
            _instance.acquisitionStarted = true;

            //Disabling start once started
            //disableEnableStartStop();

        }, function (error) {
            showMessage(error.message);
        });
    };

    FingerprintSdkTest.prototype.stopCapture = function () {
        if (!this.acquisitionStarted) //Monitor if already stopped capturing
            return;
        var _instance = this;
       // showMessage("");
        this.sdk.stopAcquisition().then(function () {
            _instance.acquisitionStarted = false;

            //Disabling stop once stoped
            //disableEnableStartStop();

        }, function (error) {
           // showMessage(error.message);
        });
    };

    FingerprintSdkTest.prototype.stopingCapture = function () {

        var _instance = this;
        showMessage("");
        this.operationToRestart = this.startCapture;
        this.sdk.stopAcquisition().then(function () {
            _instance.acquisitionStarted = false;

            //Disabling stop once stoped
            //disableEnableStartStop();

        }, function (error) {
            showMessage(error.message);
        });


    };


    FingerprintSdkTest.prototype.initAcquisition = function (){
        debugger;
        var _instance = this;
        this.operationToRestart = this.startCapture;
        return this.sdk.startAcquisition(currentFormat, myVal);
    };

    FingerprintSdkTest.prototype.getInfo = function () {
        debugger;
        var _instance = this;
        
        return this.sdk.enumerateDevices();
    };

    FingerprintSdkTest.prototype.getDeviceInfoWithID = function (uid) {
        var _instance = this;
        return this.sdk.getDeviceInfo(uid);
    };


    return FingerprintSdkTest;
})();

function showMessage(message) {
    var _instance = this;
    var statusWindow = document.getElementById("status");

    statusWindow.innerHTML = message;
    
   //// x = state.querySelectorAll("#status");
   // if (statusWindow.length != 0) {
   //     statusWindow[0].innerHTML = message;
   // }
}

function changeViewControl() {
    debugger;
    activeRightSide();

}


window.onload = function () {

    

    localStorage.clear();
    test = new FingerprintSdkTest();
    detectingReaders(true); //To populate readers for drop down selection
    localStorage.setItem("imageSrc", "");
   // disableEnable(); // Disabling enabling buttons - if reader not selected

  

};


 function onLoad() 
 {

    debugger;

     localStorage.clear();

     onStart();
     toggle_visibility(['content-capture']);
     enableDisableScanQualityDiv("content-capture"); // To enable disable scan quality div

};



function onStart() {

    debugger;

    if (currentFormat === "") {
        alert("Please select a format.");
    } else {
        test.startCapture();
    }
}

function onStop() {

    test.stopCapture();
}


function stopingCapture() {


    assignFormat();
    test.stopingCapture();

}

function startingCapture() {


    test.startingCapture();

}



function onClear() {

    var vDiv = document.getElementById('imagediv');
    vDiv.innerHTML = "";
    localStorage.setItem("imageSrc", "");
}


function onInit(event) {

    debugger;

    var vDiv = document.getElementById('imagediv');
    vDiv.innerHTML = "";
    localStorage.setItem("imageSrc", "");
    initCapture(1, event)
    unsetSideRegistered("leftPosition", "")
    unsetSideRegistered("rightPosition", "")
}

function toggle_visibility(ids) {
    debugger;
    document.getElementById("qualityInputBox").value = "";
    //onStop();
    enableDisableScanQualityDiv(ids[0]); // To enable disable scan quality div
    for (var i = 0; i < ids.length; i++) {
        var e = document.getElementById(ids[i]);
        if (i == 0) {
            e.style.display = 'block';
            state = e;
            //disableEnable();
        }

    }
}


function save() {

    //debugger;


    counterReaders = localStorage.getItem("countReaders");
    if (counterReaders > "0")
        localStorage.setItem("huella_" + counterReaders, dataToSend);

}


function sampleAcquired(s) {

    debugger;

    counterReaders = localStorage.getItem("countReaders");

    var strored = localStorage.getItem("storedFP");

    if (strored == "false" && counterReaders >= "5")
        return;

    if (currentFormat == Fingerprint.SampleFormat.PngImage) {
        // If sample acquired format is PNG- perform following call on object recieved 
        // Get samples from the object - get 0th element of samples as base 64 encoded PNG image         
        localStorage.setItem("imageSrc", "");
        var samples = JSON.parse(s.samples);
        localStorage.setItem("imageSrc", "data:image/png;base64," + Fingerprint.b64UrlTo64(samples[0]));
        if (state == document.getElementById("content-capture")) {
            var vDiv = document.getElementById('imagediv');
            vDiv.innerHTML = "";
            var image = document.createElement("img");
            image.id = "image";
            image.src = localStorage.getItem("imageSrc");
            vDiv.appendChild(image);
        }


    }

    else if (currentFormat == Fingerprint.SampleFormat.Compressed) {
        // If sample acquired format is Compressed- perform following call on object recieved 
        // Get samples from the object - get 0th element of samples and then get Data from it.
        // Returned data is Base 64 encoded, which needs to get decoded to UTF8,
        // after decoding get Data key from it, it returns Base64 encoded wsq image
        //localStorage.setItem("wsq", "");
        var samples = JSON.parse(s.samples);
        var sampleData = Fingerprint.b64UrlTo64(samples[0].Data);
        var decodedData = JSON.parse(Fingerprint.b64UrlToUtf8(sampleData));
        dataToSend = JSON.stringify(decodedData);
    }

    else if (currentFormat == Fingerprint.SampleFormat.Intermediate) {
        // If sample acquired format is Intermediate- perform following call on object recieved 
        // Get samples from the object - get 0th element of samples and then get Data from it.
        // It returns Base64 encoded feature set
        //localStorage.setItem("intermediate", "");
        var samples = JSON.parse(s.samples);
        var sampleData = samples[0].Data;
        dataToSend = sampleData;//JSON.stringify(sampleData);        
        //localStorage.setItem("intermediate", sampleData);
    }

    else if (currentFormat == Fingerprint.SampleFormat.Raw) {

        debugger;
        // If sample acquired format is RAW- perform following call on object recieved 
        // Get samples from the object - get 0th element of samples and then get Data from it.
        // Returned data is Base 64 encoded, which needs to get decoded to UTF8,
        // after decoding get Data key from it, it returns Base64 encoded raw image data
        var samples = JSON.parse(s.samples);
        var sampleData = Fingerprint.b64UrlTo64(samples[0].Data);
        var decodedData = JSON.parse(Fingerprint.b64UrlToUtf8(sampleData));
        dataToSend = JSON.stringify( decodedData);// Fingerprint.b64UrlTo64(decodedData.Data);
        

    }


    debugger;

    save();

    onCompare();

}




// Check for redirecting is a boolean value which monitors to redirect to content tab or not
function detectingReaders() {
    myVal = "";

    debugger;
    var allReaders = test.getInfo();
    

    allReaders.then(function (sucessObj) {

        debugger;

        reader = sucessObj[0];
        ////Check if readers are available get count and  provide user information if no reader available, 
        ////if only one reader available then select the reader by default and sennd user to capture tab
        checkReaderCount(sucessObj);

       

    }, function (error) {
        showMessage(error.message);
        });
}



function checkReaderCount(sucessObj)//, checkForRedirecting) {
{
    debugger;
    if (sucessObj.length == 0) 
        alert("No reader detected. Please insert a reader.");
   
}

function selectChangeEvent() {
    debugger;
    // var readersDropDownElement = document.getElementById("readersDropDown");
    myVal = reader;//readersDropDownElement.options[readersDropDownElement.selectedIndex].value;
    //disableEnable();
    onClear();
    // document.getElementById('imageGallery').innerHTML = "";


}

function disableEnrroll() {


    $('#send').prop('disabled', true);
}

function enableEnrroll() {

    if (ObjstepRecord.counter == "6" || ObjstepRecord.counter == "11") {
        $('#start').prop('disabled', true);
        $('#stop').prop('disabled', true);
        $('#send').prop('disabled', false);
    }

}


//Enable disable buttons
function disableEnable() {
    debugger;
    if (myVal != "") {
        disabled = false;
        showMessage("");
        //disableEnableStartStop();
    } else {
        disabled = true;
        showMessage("Please select a reader");
        //onStop();
    }
}


function enableDisableScanQualityDiv(id) {
    if (id == "content-reader") {
        document.getElementById('Scores').style.display = 'none';
    } else {
        document.getElementById('Scores').style.display = 'block';
    }
}

function assignFormat() {
    countReaders = localStorage.getItem("countReaders");

    if (countReaders == "0")
        currentFormat = Fingerprint.SampleFormat.PngImage;
    if (countReaders > "0")
        currentFormat = Fingerprint.SampleFormat.Intermediate;

}

function showFP() {
    if (state == document.getElementById("content-capture")) {
        var vDiv = document.getElementById('imagediv');
        vDiv.innerHTML = "";
        var image = document.createElement("img");
        image.id = "image";
        image.src = "images/huellaverde.png"
        vDiv.appendChild(image);
    }

}

function disableEnableSaveThumbnails(val) {
    if (val) {
        $('#save').prop('disabled', true);
    } else {
        $('#save').prop('disabled', false);
    }
}


function delayAnimate(id, visibility) {
    document.getElementById(id).style.display = visibility;
}


function onCompare() {

    var data = {
        ObjAttend:
        {

            nhc: $("input[name*='lblnhc']").val(),
            huella: dataToSend,
            usuarioatencion: $("input[name*='lblUsuario']").val(),
            unidadatencion: $("input[name*='lblUnidadAtencion']").val()
        }
    };
    $.ajax(
        {
            type: "POST",
            url: "/reghuellas/AtencionPaciente.aspx/AtenderPaciente",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                debugger;
                var data = JSON.parse(response.d);

                if (data.Mensaje === "success") {
                    $('.btn-primary').show();

                    $("#mensaje").removeClass("alert-danger");
                    $("#mensaje").addClass("alert-success");
                    $("#mensaje").css("display", "block");
                    $("#mensaje").attr("style", "display:block");
                    $("#mensaje").css("visibility", "visible");
                    $("#mensaje").val(data.DetalleError);
                    showMessage("");
                    showFP();

                }

                else {
                    $("#mensaje").removeClass("alert-success");
                    $("#mensaje").addClass("alert-danger");
                    $("#mensaje").css("display", "block");
                    $("#mensaje").attr("style", "display:block");
                    $("#mensaje").css("visibility", "visible");
                    $("#mensaje").val(data.DetalleError);
                }

                onStop();

            },
            failure: function (response) {
                alert(response.d);
            }
        });
}

function onExludePatient()
{

    var data = {
        ObjExclude:
        {

            nhc: $("input[name*='lblnhc']").val(),            

        }
    };
    
    return $.ajax(
    {
        type: "POST",
            url: "/reghuellas/ExcluirPacientesCircuito.aspx/EvaluarExclusion",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
        //success: function (response) {

        //    debugger;
        //    var data = JSON.parse(response.d);

                   
        //},
        //failure: function (response) {
        //    alert(response.d);
        //}
    });
}
function onSuccess(e) { }

