
/* Button Load more - v1.0.0 - 2018-02-28
* Copyright (c) 2018 NTTPS; */

(function ($) {
    $.fn.btnLoadmore = function (options) {
        var defaults = {
            showItem: 6,
            whenClickBtn: 3,
            textBtn: 'مشاهده محصولات بیشتر',
            classBtn: '',
            setCookies: false,
            delayToScroll: 2000,

        },
            options = $.extend(defaults, options);

        this.each(function () {

            var $this = $(this),
                $childrenClass = $($this.children());

            // Get Element Of contents to hide
            $childrenClass.hide();

            //Show Element from Options
            $childrenClass.slice(0, defaults.showItem).show();

            //Show Button when item in contents != 0
            //if ($childrenClass.filter(":hidden").length > 0) {
            $this.after('<button type="button" class="btn-loadmore ' + defaults.classBtn + '">' + defaults.textBtn + '</button>')
            //}
            var page = 1;
            $(document).on('click', '.btn-loadmore', function (e) {
                e.preventDefault();
                page++;

                var sortby = getUrlVars()["sortby"];
               
                var url = window.location.pathname;
                var brand = decodeURIComponent(url.substring(url.lastIndexOf('/') + 1));

                $.ajax(
                    {
                        url: "/products/GetNewPageForBrand",
                        data: { page: page, sort: sortby, brand: brand },
                        type: "GET",
                        success: function (json) {
                           
                            for (var i = 0; i < json.Result.length; i++) {
                                var card = getItem(json.Result[i].Code,
                                    json.Result[i].Title,
                                    json.Result[i].Amount,
                                    json.Result[i].ImageUrl,
                                    json.Result[i].Stock,
                                    json.Result[i].DiscountAmount);
                                
                                $(".shop_container").append(card);
                            }

                            $childrenClass.filter(':hidden').slice(0, defaults.whenClickBtn).slideDown();
                            //if ($childrenClass.filter(":hidden").length == 0) {
                            //    $(".btn-loadmore").fadeOut('slow');
                            //}
                            if (json.IsLastBatch === "True") {
                                $(".btn-loadmore").fadeOut('slow');
                            }
                            scrollDown(page - 1); 
                        }
                    });





            });

            function scrollDown(page) {
                $('html, body').animate({
                    scrollTop: page * $childrenClass.filter(":visible").last().offset().top
                }, defaults.delayToScroll);
            }
        });



    }
}(jQuery));