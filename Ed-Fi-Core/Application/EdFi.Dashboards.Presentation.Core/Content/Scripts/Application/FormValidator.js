// this is a custom rule for making a field required
var RequiredValidationRule = function () {
    this.Rule = {
        required: true
    };
};

// this is a custom rule for validating a minimum and maximum range
var RangeValidationRule = function(minValue, maxValue, minMessage, maxMessage, isRequired) {
    this.Rule = {
        min: minValue,
        max: maxValue,
        required: isRequired,
        message: {
            min: minMessage,
            max: maxMessage
        }
    };
};

// this object will be used to contain validation routines to a single object
// so that the actual validation methods can be changed out without having to
// change code everywhere in the application
var FormValidator = function(formId) {
    var self = this,
        formIdentifier = "";

    if (formId !== "") {
        formIdentifier = formId.contains("#") ? formId : "#" + formId;
    }

    this.IsFormValid = true;

    this.initializeValidator = function () {
        // override the errorPlacement and success events so we can create some
        // custom functionality
        jQuery.validator.setDefaults({
            errorPlacement: function (error, element) {
                var message = error.text();

                if (message !== "") {
                    if ($("#" + error.attr("for") + "-icon").attr("id") === undefined) {
                        element.attr("title", message);

                        var errorIcon = $("<i />");
                        errorIcon.attr("id", error.attr("for") + "-icon");
                        errorIcon.attr("class", "icon-asterisk error-icon");
                        errorIcon.insertAfter(element);
                    }

                    self.IsFormValid = false;
                } else {
                    self.IsFormValid = true;
                }
            },
            success: function (error, element) {
                if ($(element).attr("id") !== undefined) {
                    var elementIdentifier = "#" + $(element).attr("id");
                    if ($(elementIdentifier).attr("title") !== undefined) {
                        $(elementIdentifier).removeAttr("title");
                    }
                }

                var errorIconIdentifier = "#" + error.attr("for") + "-icon";
                if ($(errorIconIdentifier).attr("id") !== undefined) {
                    $(errorIconIdentifier).remove();
                }

                self.IsFormValid = true;
            }
        });
        // this is called to initialize the jquery validator
        $(formIdentifier).validate();
    };

    // adds a validation rule for a single element on a form
    this.addValidationMethod = function(elementId, validationObject) {
        var elementIdentifier = elementId.contains("#") ? elementId : "#" + elementId;

        var validationMethod;
        if (validationObject.Rule !== undefined) {
            validationMethod = validationObject.Rule;
        } else {
            validationMethod = validationObject;
        }

        if (validationMethod !== undefined) {
            $(elementIdentifier).rules("add", validationMethod);
        }
    };

    // adds a validation rule for a list of elements on a form
    this.addValidationMethodForSelection = function(selectionText, validationObject) {
        $(selectionText).each(function() {
            var selectionIdValue = $(this).prop("id");

            self.addValidationMethod(selectionIdValue, validationObject);
        });
    };

    // this causes the validation routine to fire
    this.validate = function() {
        $(formIdentifier).validate();
    };
};