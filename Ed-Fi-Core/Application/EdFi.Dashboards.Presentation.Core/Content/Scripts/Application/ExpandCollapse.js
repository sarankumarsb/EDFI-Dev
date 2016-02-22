/// <reference path="../External/jquery.js" />

$(document).on('click', '.accordion-toggle', function (e) {
	e.preventDefault();
	
	var $link = $(this);
	var $target = $($link.data('target'));
    $link.find(".icon").toggle();
	$target.toggle();
});

function CollapseToggleDiv(imgId, divId) {
	$(divId).hide();
    $(imgId).find(".icon").toggle();
}