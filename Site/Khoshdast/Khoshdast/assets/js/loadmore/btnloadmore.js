
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
                $('.loading-img').css('display', 'block');
                $('.newclass').css('display', 'block');
                var sortby = getUrlVars()["sortby"];
                var brands = "";


                for (var i = 0; i < 1000; i++) {
                    var currentBrand = getUrlVars()["brands[" + i + "]"];
                    if (currentBrand !== undefined) {
                        brands += "-" + currentBrand;
                    } else {
                        break;
                    }
                }

                var url = window.location.pathname;
                var category = decodeURIComponent(url.substring(url.lastIndexOf('/') + 1));

                $.ajax(
                    {
                        url: "/products/GetNewPage",
                        data: { page: page, sort: sortby, brands: decodeURIComponent(brands), category: category },
                        type: "GET",
                        success: function (json) {
                            //$('.newclass').removeClass('newclass');
                            for (var i = 0; i < json.Result.length; i++) {
                                var card = getItem(json.Result[i].Code,
                                    json.Result[i].Title,
                                    json.Result[i].Amount,
                                    json.Result[i].ImageUrl,
                                    json.Result[i].Stock);
                                
                                $(".shop_container").append(card);
                            }

                            $childrenClass.filter(':hidden').slice(0, defaults.whenClickBtn).slideDown();
                            //if ($childrenClass.filter(":hidden").length == 0) {
                            //    $(".btn-loadmore").fadeOut('slow');
                            //}
                            if (json.IsLastBatch === "True") {
                                $(".btn-loadmore").fadeOut('slow');
                            }
                            $('.loading-img').css('display', 'none');
                            //document.getElementById(".col-md-3").style.visibility = "visible";
                             scrollDown(page-1); 
                        }
                    });
            });

            function scrollDown(page) {
                var aa = $childrenClass.filter(":visible").last().offset().top;
                $('html, body').animate({
                    scrollTop:page* $childrenClass.filter(":visible").last().offset().top
                }, defaults.delayToScroll);
            }
        });



    }
}(jQuery));