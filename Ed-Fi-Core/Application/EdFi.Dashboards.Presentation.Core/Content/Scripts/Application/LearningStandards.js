(function ($) {

    $.fn.learningStandards = function (options) {

        return this.each(function () {

            var $this = $(this).find('.learning-standards-table');

            var resizeRows = function () {
                $this.find('.leftPane-content tbody tr:visible').each(function () {
                    var $row = $(this);
                    var index = $this.find('.leftPane-content tbody tr').index($row);
                    var $pairedRow = $($this.find('.rightPane-content tbody tr').eq(index));

                    var desiredHeight = Math.max($row.height(), $pairedRow.height());

                    $row.height(desiredHeight);
                    $pairedRow.height(desiredHeight);
                });
            };

            // Body

            var highlightRow = function (i, ele) {
                var row = $(ele);
                if (i % 2 == 0) {
                    row.addClass("highlighted");
                } else {
                    row.removeClass("highlighted");
                }
            };

            // Actions
            $this.on('click', 'a', function (e) {
                e.preventDefault();

                var $currentRow = $(this).closest('tr');
                $currentRow.toggleClass('collapsed');
                var hide = $currentRow.hasClass('collapsed');
                $currentRow.trigger('toggleHeaders', hide);
                var parentColonCount = $currentRow.attr('data-tag').split(":").length - 1;

                $this.find("tr[data-tag^='" + $currentRow.data('tag') + ":']").each(function () {
                    var currentTr = $(this);
                    var childColonCount = currentTr.attr('data-tag').split(":").length - 1;
                    //If we are hiding make sure to hide all children...
                    if (!hide) {
                        currentTr.trigger('hide', hide);
                        currentTr.removeClass('collapsed');
                    } else { //Else just open the first child below.
                        if (childColonCount - parentColonCount == 1) {
                            currentTr.trigger('hide', hide);
                        }
                    }
                });

                $currentRow.trigger('toggleAssessments');

                resizeRows();
            });


            $this.find('tr').on('hide', function (e, hide) {
                $(this).toggle(hide);
                $this.find(".leftPane tbody tr:visible").each(function (i, ele) { highlightRow(i, ele); });
                $this.find(".rightPane tbody tr:visible").each(function (i, ele) { highlightRow(i, ele); });
            });

            $this.find('tr').on('toggleHeaders', function (e, show) {
                var linktoheaders = $(this).data('linktoheaders');
                if (linktoheaders === '') return;

                if (show) {
                    $this.find("[data-tag^='" + linktoheaders + "']").toggle(show);
                } else {
                    var otherRows = $this.find("[data-linktoheaders^='" + linktoheaders + "'].collapsed");
                    if (otherRows.length === 0) {
                        $this.find("[data-tag^='" + linktoheaders + "']").hide();
                    }
                }

            });

            $this.find('tr').on('toggleAssessments', function () {

                var allItems = $this.find("[data-tag*='Grade:assessment']");

                //Trigger to show or hide is based on the level-1 being open.
                var showAssessmentHeaders = $(".level-1.collapsed").length > 0;

                if (showAssessmentHeaders)
                    allItems.show();
                else
                    allItems.hide();
            }
            );

            //Scrolling Sync
            $this.find('.rightPane').scroll(function () {
                var left = $this.find('.rightPane').scrollLeft();
                var top = $this.find('.rightPane').scrollTop();

                $this.find('.rightPaneHeaders').scrollLeft(left);
                $this.find('.scroller').scrollTop(top);
            });

            $this.find('.scroller').scroll(function () {
                var top = $this.find('.scroller').scrollTop();
                $this.find('.rightPane').scrollTop(top);
            });

            // Initialize
            $this.find('tbody tr').trigger('toggleHeaders', false);
            $this.find('tbody tr').not('.level-0').hide();

        });

    };

    var $container = $(".learning-standard"),
        $fullscreen = $(".maximize-button");

    $fullscreen.click(function (e) {
        e.preventDefault();
        $container.toggleClass('fullscreen');
    });

}(jQuery));

