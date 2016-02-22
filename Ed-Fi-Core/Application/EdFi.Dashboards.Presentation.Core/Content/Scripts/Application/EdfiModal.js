(function ($) {
    $.fn.edfiStandardModal = function () {
        //Init:

        this.openModal = function () {

            //Inserts the modal content into the modal shell
            $(".modal-wrapper").children().each(function() {
                $("#modal").append($(this));
            });

            //Displays the modal after the content is added
            $("#modal-container").removeClass("hide-modal");
        };

        this.closeModal = function () {
            
            //Hides the modal and clears inputs when the modal is closed
            $("#modal-container").addClass("hide-modal");
        };
        
        return this;
    };
}(jQuery));