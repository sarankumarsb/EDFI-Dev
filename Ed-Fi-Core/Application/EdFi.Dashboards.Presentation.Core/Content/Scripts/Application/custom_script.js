/*Custome script*/
$(document).ready(function () {
    var win_width = $(window).width();
    var isMobile = false;

    if (win_width <= 768) {
        isMobile = true;
    }


    if (isMobile) {
        //menu icon position
        adjustMenuIconPosition();
    }
    else {
        //assign submenu width
        assignSubmenuItemsWidth();
    }

    $('.std-secondary-title').click(function () {
        if (isMobile) {
            $(this).next().slideToggle(300);
        }
    });


    function assignSubmenuItemsWidth() {
        var submenu_holder = $('.overlay_menu_wrapper').find('.holder');
        var submenu_ul = $(submenu_holder).find('ul');
        var parent_width = $('.nav-tabs').width();
        var total_children = $(submenu_ul).find('li').length;
        var li_width = parent_width / total_children;
        $(submenu_ul).find('li').css({ 'width': (li_width - 3) + 'px' });
    }

    //adjust menu icon position to middle of tab
    function adjustMenuIconPosition() {
        var tab_width = $('.nav-tabs .tab1').width();
        var icon_width = $('.overlay_menu_wrapper .icon').width();
        var left_val = (tab_width / 2) - ((icon_width / 2) + 5);
        $('.overlay_menu_wrapper .icon').css('left', left_val + 'px');
    }

    //popupmenu - script
    $('.overlay_menu_wrapper .icon').click(function () {
        $('.overlay_menu_wrapper .holder').slideToggle(300, function () {
            if ($('.overlay_menu_wrapper .holder').is(':visible')) {
                $('.overlay_menu_wrapper .icon').attr('src', '/EdFiDevNew/App_Themes/Theme1/img/menu_up_arrow.png');
                $('.overlay_menu_wrapper').css({ 'height': ($(document).height()) + 'px' });
                $('.overlay_menu_wrapper .icon_holder').css('background-color', '#eee');
            }
            else {
                $('.overlay_menu_wrapper').css({ 'height': '0px' });
                $('.overlay_menu_wrapper .icon').attr('src', '/EdFiDevNew/App_Themes/Theme1/img/menu_down_arrow.png');
                $('.overlay_menu_wrapper .icon_holder').css('background-color', '#fff');
            }
        });
    });

    //on Resize - reset menu
    $(window).on('resize', function () {
        if ($(window).width() >= 991) {
            $('.constrained .l-tabs ul.sub_ul1').css({ 'position': 'absolute', 'width': '100%' });
            $('.overlay_menu_wrapper').css({
                'height': '61px',
                'display': 'block'
            });
            $('.overlay_menu_wrapper .holder').css({ 'display': 'block' });

            //assign submenu width
            assignSubmenuItemsWidth();
        }
        else {
            $('.overlay_menu_wrapper').css({ 'height': '0px' });
            $('.overlay_menu_wrapper .icon').attr('src', '/EdFiDevNew/App_Themes/Theme1/img/menu_down_arrow.png');
            $('.overlay_menu_wrapper .icon_holder').css('background-color', '#fff');
            $('.overlay_menu_wrapper .holder').css({ 'display': 'none' });

            //menu icon position
            adjustMenuIconPosition();
        }
    });

});
