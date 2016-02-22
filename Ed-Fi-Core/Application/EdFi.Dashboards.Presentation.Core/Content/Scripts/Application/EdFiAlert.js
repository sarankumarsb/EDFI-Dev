(function ($) {

    $.fn.edFiStandardAlert = function () {

        //Init:
        $(".edfi-alert-close").on("click", function () {
            $("#edfi-standard-alert").slideUp();
        });

        //Properties:

        //Available status
        this.status = {
            success: { cssClass: "edfi-alert-success", text: "Success:" },
            error: { cssClass: "edfi-alert-error", text: "Error:" },
            warning: { cssClass: "edfi-alert-warning", text: "Warning:" },
            info: { cssClass: "edfi-alert-info", text: "Info:" }
        };

        //Methods:
        this.show = function (edFiStandardAlertStatus, message, redirectUrl) {
            if ($("#edfi-standard-alert").is(":visible")) {
                $("#edfi-standard-alert").slideUp(function () {
                    $("#edfi-standard-alert").removeClass();
                    showNow(edFiStandardAlertStatus, message, redirectUrl);
                });
            } else {
                showNow(edFiStandardAlertStatus, message, redirectUrl);
            }
        };
        
        this.showDelayHide = function (edFiStandardAlertStatus, message, redirectUrl) {
            this.show(edFiStandardAlertStatus, message, redirectUrl);
            setTimeout(function () {
                $("#edfi-standard-alert").slideUp();
            }, 3000);
        };

        this.hide = function () {
            $("#edfi-standard-alert").slideUp();
        };
        
        this.hideDelay = function () {
            setTimeout(function () {
                $("#edfi-standard-alert").slideUp();
            }, 2500);
        };

        function showNow(edFiStandardAlertStatus, message, redirectUrl) {
            $("#edfi-standard-alert").addClass(edFiStandardAlertStatus.cssClass);
            $("#edfi-alert-status").text(edFiStandardAlertStatus.text);
            $("#edfi-alert-message").text(message);
            $("#edfi-standard-alert").slideDown();

            if (redirectUrl) {
                setTimeout(function () {
                    window.location = redirectUrl;
                }, 2500);
            }
        }

        return this;
    };

}(jQuery));