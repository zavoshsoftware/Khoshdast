function SubmitComment() {

     
    var url = window.location.pathname;
    var id = url.substring(url.lastIndexOf('/') + 1);

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/ProductComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, code: id },
                type: "GET"
            }).done(function (result) {
            if (result === "true") {
                $("#errorDiv").css('display', 'none');
                $("#SuccessDiv").css('display', 'block');
                localStorage.setItem("id", "");
            }
            else if (result === "InvalidEmail") {
                $("#errorDiv").html('ایمیل وارد شده صحیح نمی باشد.');
                $("#errorDiv").css('display', 'block');
                $("#SuccessDiv").css('display', 'none');

            }
        });
    }
    else {
        $("#errorDiv").html('تمامی فیلد های زیر را تکمیل نمایید.');
        $("#errorDiv").css('display', 'block');
        $("#SuccessDiv").css('display', 'none');

    }
}


function addDiscountCode() {
    var coupon = $("#coupon").val();
    $('#errorDiv-discount').css('display', 'none');
    if (coupon !== "") {
        $.ajax(
            {
                url: "/Basket/DiscountRequestPost",
                data: { coupon: coupon },
                type: "GET"
            }).done(function (result) {
                if (result !== "Invald" && result !== "Used" && result !== "Expired") {
                    location.reload();
                }
                else if (result !== true) {
                    $('#errorDiv-discount').css('display', 'block');
                    if (result.toLowerCase() === "used") {
                        $('#errorDiv-discount').html("این کد تخفیف قبلا استفاده شده است.");
                    }
                    else if (result.toLowerCase() === "expired") {
                        $('#errorDiv-discount').html("کد تخفیف وارد شده منقضی شده است.");
                    }
                    else if (result.toLowerCase() === "invald") {
                        $('#errorDiv-discount').html("کد تخفیف وارد شده معتبر نمی باشد.");
                    }
                    else if (result.toLowerCase() === "true") {
                        $('#SuccessDiv-discount').css('display', 'block');
                        $('#errorDiv-discount').css('display', 'none');
                    }
                }
            });

    } else {
        $('#SuccessDiv-discount').css('display', 'none');
        $('#errorDiv-discount').html('کد تخفیف را وارد نمایید.');
        $('#errorDiv-discount').css('display', 'block');
    }
}

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function addToBasket(code, qty) {
    if (qty === 'detail') {
        qty = $('#basketQty').val();
    }

    $.ajax(
        {
            url: "/cart",
            data: { code: code, qty: qty },
            type: "Post"
        }).done(function (result) {
            if (result !== true) {
                window.location = "/basket";
            }
        });
}

function FinalizeOrder() {
    //DisappearButton();
    var orderNotes = $('#orderNotes').val();
    var billing_cellnumber = $('#billing_cellnumber').val();
    var billing_postal = $('#billing_postal').val();
    var billing_Address = $('#billing_Address').val();
    var billing_city = $('#billing_city option:selected').val();
    var billing_fullname = $('#billing_fullname').val();
    var biling_paymentType = '';
    if ($("#offline-payment").prop("checked")) {
        biling_paymentType = 'offline';
    }
    else if ($("#online-payment").prop("checked")) {
        biling_paymentType = 'online';
    }

    if (billing_city === undefined || billing_city === '0' || billing_city === 0)
        billing_city = "";
    if (billing_fullname !== "" &&
        billing_cellnumber !== "" &&
        billing_Address !== "" &&
        billing_city !== "") {
        $.ajax(
            {
                url: "/Basket/Finalize",
                data: {
                    notes: orderNotes, cellnumber: billing_cellnumber, postal: billing_postal, address: billing_Address,
                    city: billing_city, fullname: billing_fullname, paymentType: biling_paymentType
                },
                type: "GET"
            }).done(function (result) {
                if (result.includes('nonstock')) {
                    var products = result.split('|');
                    var productNames = "";
                    for (var i = 1; i < products.length - 1; i++) {
                        productNames = productNames + "\"" + products[i] + "\"";
                    }

                    $('#error-box').css('display', 'block');
                    $('#error-box').html('کاربر گرامی موجودی کالای ' +
                        productNames +
                        'از موجودی کالا انتخابی شما کمتر می باشد. لطفا به ' +
                        '<a href="/basket">سبد خرید </a>' +
                        ' خود مراجعه کنید و موجودی را تغییر دهید.');
                    AppearButton();

                }
                else if (result !== "false") {
                    window.location = result;
                }
                else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    AppearButton();

                }
            });
    }
    else {
        $('#error-box').css('display', 'block');
        $('#error-box').html('تمامی فیلدهای ستاره دار باید تکمیل شود');
        //AppearButton();

    }

}

function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function updateBasket() {
    var cookie = getCookie('basket-khoshdast');

    var orderDetails = cookie.split('/');
    var newOrderDetails = '';

    for (var i = 0; i < orderDetails.length - 1; i++) {
        var orderDetail = orderDetails[i].split('^');

        var txtQty = $('#qty_' + orderDetail[0]).val();

        if (txtQty !== orderDetail[1]) {
            orderDetails[i] = orderDetail[0] + '^' + txtQty;
        }
        newOrderDetails = newOrderDetails + orderDetails[i] + '/';
    }

    deleteCookie('basket-khoshdast');
    setcookie('basket-khoshdast', newOrderDetails, 100);
    location.reload();
}

function search() {
    var searchTerm = $('#TextSearch').val();

    //window.location = "/product?searchterm=" + searchTerm;

    //window.location.replace("/product?searchterm=" + searchTerm);
    //return false;


    window.onload = function () {
        document.getElementById("search-form").onsubmit = function () {
            window.location.replace("/product?searchterm=" + searchTerm);
            return false;
        }
    }
}

function postNewsletter() {
    var newsletter = $('#txtNewsletter').val();
    if (newsletter === '') {
        $('#danger-alert-nl').css('display', 'block');
        $('#success-alert-nl').css('display', 'none');
        $('#danger-alert-nl').html('ایمیل خود را وارد نمایید');
    } else {
        $.ajax(
            {
                url: "/Newsletters/EmailPost",
                data: { email: newsletter },
                type: "Post"
            }).done(function (result) {
                if (result === "true") {
                    $('#danger-alert-nl').css('display', 'none');
                    $('#success-alert-nl').css('display', 'block');
                } else if (result == "invalidemail") {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('ایمیل وارد شده صحیح نمی باشد');
                } else {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('خطایی رخ داده است. لطفا دقایقی دیگر مجدادا تلاش کنید');
                }
            });
    }
}


function SubmitBlogComment(id) {
    
    //var url = window.location.pathname;
    //var id = url.substring(url.lastIndexOf('/') + 1);

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    var siteVal = $("#commentSite").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/BlogComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, code: id, site:siteVal },
                type: "POST"
            }).done(function (result) {
            if (result === "true") {
                $("#errorDivBlog").css('display', 'none');
                $("#SuccessDivBlog").css('display', 'block');
                localStorage.setItem("id", "");
            }
            else if (result === "InvalidEmail") {
                $("#errorDivBlog").html('ایمیل وارد شده صحیح نمی باشد.');
                $("#errorDivBlog").css('display', 'block');
                $("#SuccessDivBlog").css('display', 'none');

            }
        });
    }
    else {
        $("#errorDivBlog").html('تمامی فیلد های زیر را تکمیل نمایید.');
        $("#errorDivBlog").css('display', 'block');
        $("#SuccessDivBlog").css('display', 'none');

    }
}

function SubmitContactUs() {

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/ContactUsForms/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal },
                type: "POST"
            }).done(function (result) {
            if (result === "true") {
                $("#errorDivContact").css('display', 'none');
                $("#SuccessDivContact").css('display', 'block');
                localStorage.setItem("id", "");
            }
            else if (result === "InvalidEmail") {
                $("#errorDivContact").html('ایمیل وارد شده صحیح نمی باشد.');
                $("#errorDivContact").css('display', 'block');
                $("#SuccessDivContact").css('display', 'none');

            }
        });
    }
    else {
        $("#errorDivContact").html('تمامی فیلد های بالا را تکمیل نمایید.');
        $("#errorDivContact").css('display', 'block');
        $("#SuccessDivContact").css('display', 'none');

    }
}
