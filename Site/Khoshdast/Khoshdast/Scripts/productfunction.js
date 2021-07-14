function freezePage() {
    $("#loading").addClass('modalWindow');
    $("#loading img").css('display', 'inline-block');
}

function unFreezePage() {
    $("#loading").removeClass('modalWindow');
    $("#loading img").css('display', 'none');
}

function loadProductList(products) {
    var item = "";
    for (var i = 0; i < products.length; i++) {
        item = item + getProductItem(products[i].Id, products[i].Title, products[i].ImageUrl, products[i].Amount);
    }
    $('#product-list').html(item);
}

function getProductItem(id, title, image, amount) {

    var item = "<div class='col-md-4'><div><img class='img-responsive' src='" +
        image +
        "'/></div><div>" +
        title +
        "</div><div>" +
        amount +
        "</div><div><input type='button' value='انتخاب' class='btn btn-primary' style='margin-top:5px;'  onclick=addToBasket('" +
        id + "'); />" +
        "</div></div>";
    return item;
}

function addToBasket(id) {
    freezePage();
    
   
        addProductToCookie(id);
    
        $.ajax({
            type: "GET",
            url: "/Pos/GetBasketInfoByCookie",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {

                var item = "";
                for (var i = 0; i < data.Products.length; i++) {
                    item = item +
                        loadBasket(data.Products[i].Title,
                            data.Products[i].Quantity,
                            data.Products[i].Amount,
                            data.Products[i].RowAmount,
                            data.Products[i].Id);
                }

                $('#factor tbody').html(item);
                $('#total').html(data.Total);
                $('#totalAmount').html(data.Total);
                $('#remainAmount').html(data.Total);
                //$('#totalAmount').val(data.Total); 
          /*      changeTotal();*/
                changeTotalOrder();
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });

    unFreezePage();
}

function addProductToCookie(id) {
    var currentBasket = '';
    var currentCookie = getCookie("basket");

    

    if (currentCookie.includes(id)) {

        var cookieItems = currentCookie.split('/');

        var newcookie = '';

        var isSetNewPro = false;

        for (var i = 0; i < cookieItems.length - 1; i++) {
            if (cookieItems[i].includes(id)) {
                var finalcookieItem = cookieItems[i].split('^');

        
                    var qty = parseInt(finalcookieItem[1]) + 1;
                cookieItems[i] = id + "^" + qty;
                    isSetNewPro = true;
                    newcookie = newcookie + cookieItems[i] + "/";
                    //break;

             
            } else {
                newcookie = newcookie + cookieItems[i] + "/";
            }
        }

        if (!isSetNewPro) {
            newcookie = currentCookie + id + "^1./";
        }

        currentBasket = newcookie;

    } else {
        currentBasket = currentCookie + id + "^1/";
    }
    setCookie("basket", currentBasket);
}

function setCookie(name, value) {
    document.cookie = name + "=" + (value || "") + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return "";
}

function loadBasket(Title, quantity, amount, rowAmount, id) {
    //var ddl = loadClorDropDown(childProducts, id);

    // var ddlMattress = loadMattressDropDown(hasMattress, matresses, id);

    var item = "<tr>" +
        "<td>" +
        Title +
        "</td>" +
        "<td class='qtytable'>" + quantity +
        //"<td class='qtytable'><input class='qty' type='text' value=" + quantity + " id='qty" + id + "' onKeyUp='return changeRowTotal(this.id,3)'  />" +
        "</td>" +
        "<td class='amounttable'>" + amount +
        //"<td class='amounttable'><input type='text' value=" + amount + " id='amount" + id + "' onKeyUp='return changeRowTotal(this.id,6)'/>" +
        //"<td>" + amount +
        "</td>" +
        "<td id='rowAmount" + id + "'>" +
        rowAmount +
        "</td> " +
        "<td><i class='fa fa-remove' onclick=removeRow('" + id + "','PrintFactor'); />" +
        "</td>" + "</tr>";

    return item;
}

function removeRow(id,viewName) {
    freezePage();
    $.ajax({
        type: "GET",
        url: "/Pos/RemoveFromBasket",
        contentType: "application/json; charset=utf-8",
        data: { "id": id },
        datatype: "json",
        success: function (data) {
            console.log(data.Products);
            var item = "";
            for (var i = 0; i < data.Products.length; i++) {
                if (viewName === "PrintFactor") {
                    item = item + loadBasket(data.Products[i].Title,
                        data.Products[i].Quantity, data.Products[i].Amount,
                        data.Products[i].RowAmount, data.Products[i].Id);
                }
                else if (viewName === "AddOrder") {
                    item = item + loadBasketWithoutAmont(data.Products[i].Title,
                        data.Products[i].Quantity,

                        data.Products[i].Id);
                }
            }

            $('#factor tbody').html(item);
            $('#total').html("جمع کل: " + data.Total);

            //my change
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    unFreezePage();
}

function loadBasketWithoutAmont(Title, quantity, id) {
    //var ddl = loadClorDropDown(childProducts, id);

    // var ddlMattress = loadMattressDropDown(hasMattress, matresses, id);

    var item = "<tr>" +
        "<td>" +
        Title +
        "</td>" +
        "<td class='qtytable'>" + quantity +
        //"<td class='qtytable'><input class='qty' type='text' value=" + quantity + " id='qty" + id + "' onKeyUp='return changeRowTotal(this.id,3)'  />" +
        "</td>" +

        //"<td class='amounttable'><input type='text' value=" + amount + " id='amount" + id + "' onKeyUp='return changeRowTotal(this.id,6)'/>" +
        //"<td>" + amount +

        "<td><i class='fa fa-remove' onclick=removeRow('" + id + "','AddOrder'); />" +
        "</td>" + "</tr>";

    return item;
}

function addToBasketWithoutAmont(id) {
    freezePage();
    
        addProductToCookie(id);//my change add new parameter type

        $.ajax({
            type: "GET",
            url: "/Pos/GetBasketInfoByCookie",
            contentType: "application/json; charset=utf-8",
            data: { },
            datatype: "json",
            success: function (data) {

                var item = "";
                for (var i = 0; i < data.Products.length; i++) {
                    item = item +
                        loadBasketWithoutAmont(data.Products[i].ParentTitle,
                            data.Products[i].ChildTitle,
                            data.Products[i].Quantity,

                            data.Products[i].Id);
                }

                $('#factor tbody').html(item);
                $('#total').html(data.Total);
                $('#totalAmount').html(data.Total);
                $('#remainAmount').html(data.Total);
                //$('#totalAmount').val(data.Total); 
          /*      changeTotal();*/
                changeTotalOrder();
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    
    unFreezePage();
}

function changeTotalOrder() {
    
    $('#totalAmount').val('');
    var total = $('#total').html();
    
    var addedAmount = $('#addedAmount').val();
    var decreasedAmount = $('#decreasedAmount').val();
    var payment = $('#payment').val();
    if ($('#addedAmount').val().length === 0)
        addedAmount = 0;
    if ($('#decreasedAmount').val().length === 0)
        decreasedAmount = 0;
    if ($('#payment').val().length === 0)
        payment = 0;
    total = clearAmount(total);
    total = clearAmount(total);
    total = clearAmount(total);

    addedAmount = clearAmount(addedAmount);
    addedAmount = clearAmount(addedAmount);
    addedAmount = clearAmount(addedAmount);

    decreasedAmount = clearAmount(decreasedAmount);
    decreasedAmount = clearAmount(decreasedAmount);
    decreasedAmount = clearAmount(decreasedAmount);

    payment = clearAmount(payment);
    payment = clearAmount(payment);
    payment = clearAmount(payment);



    var tot = parseInt(total) + parseInt(addedAmount) - parseInt(decreasedAmount);
    var remain = parseInt(tot) - parseInt(payment);

    $('#payment').val(commafy(payment));
    $('#decreasedAmount').val(commafy(decreasedAmount));
    $('#addedAmount').val(commafy(addedAmount));
    $('#totalAmount').val(commafy(tot));
    $('#remainAmount').val(commafy(remain));
}

function clearAmount(amount) {
    if (amount.includes('تومان'))
        amount = amount.replace('تومان', '');

    if (amount.includes(','))
        amount = amount.replace(',', '');

    return amount;
}

function commafy(num) {
    var str = num.toString().split('.');
    if (str[0].length >= 5) {
        str[0] = str[0].replace(/(\d)(?=(\d{3})+$)/g, '$1,');
    }
    if (str[1] && str[1].length >= 5) {
        str[1] = str[1].replace(/(\d{3})/g, '$1 ');
    }
    return str.join('.');
}

function clearForm() {
    $('#CellNumber').val('');
    $('#fullName').val('');
    $('#address').val('');
    $('#Phone').val('');
    $('#ShipmentTypeId').val('');
    $('#addedAmount').val('0');
    $('#decreasedAmount').val('0');
    $('#payment').val('0');
    $('#remainAmount').val('0');
    $('#totalAmount').val('0');

    $('#factor tbody').html('');
    $('#total').html('0');
    setCookie("basket", '');

    $('.panel-body input').css('border-color', '#d9d9d9');
}

function finalizeOrder() {

    freezePage();

    var cookie = getCookie('basket');

    if (cookie) {
        
        var orderDate = $('#Order_OrderDateStr').val();
        var cellNumber = $('#Order_DeliverCellNumber').val();
        var fullName = $('#Order_DeliverFullName').val();
        var address = $('#Order_Address').val();
        var addedAmount = $('#addedAmount').val();
        var decreasedAmount = $('#decreasedAmount').val();
        var desc = $('#desc').val();
        var paymentTypeId = $('#PaymentTypeId').val();
        var paymentAmount = $('#payment').val();
        var subtotalAmount = $('#total').val();
        var totalAmount = $('#totalAmount').val();

        var paymentTypeIsRequired = null;

        if (paymentAmount === '0')
            paymentTypeIsRequired = "true";

        else if (paymentAmount !== '0' && paymentTypeId)
            paymentTypeIsRequired = "true";

        var img = getCookie('image');

        if (cellNumber && fullName && paymentTypeIsRequired && paymentTypeId) {
            $.ajax({
                type: "Post",
                url: "/Pos/PostFinalize",
                data: {
                    
                    "orderDate": orderDate,
                    "cellNumber": cellNumber,
                    "fullName": fullName,
                    "address": address,
                    "addedAmount": addedAmount,
                    "decreasedAmount": decreasedAmount,
                    "desc": desc,
                    "paymentAmount": paymentAmount, "paymentTypeId": paymentTypeId,
                    "subtotalAmount": subtotalAmount,
                    "totalAmount": totalAmount
                },
                success: function (data) {
                    if (data.includes("true")) {
                        var orderCode = data.split('-')[1];
                        $('#submit-succes').css('display', 'block');
                        $('#submit-succes').html('فاکتور شماره ' + orderCode + ' با موفقیت ثبت گردید.');
                        $('#submit-error').css('display', 'none');
                        //clearForm();
                        //window.location = "/orders/list";
                    } else {
                        $('#submit-succes').css('display', 'none');
                        $('#submit-error').css('display', 'block');
                        $('#submit-error').html('خطایی رخ داده است. لطفا دوباره تلاش کنید');

                    }

                },
                error: function () {
                    $('#submit-succes').css('display', 'none');
                    $('#submit-error').css('display', 'block');
                    $('#submit-error').html('خطایی رخ داده است. لطفا دوباره تلاش کنید');
                }
            });


        } else {
            $('#submit-succes').css('display', 'none');
            $('#submit-error').css('display', 'block');
            $('#submit-error').html('فیلدهای ستاره دار را تکمیل نمایید.');
            if (!paymentTypeIsRequired)
                $('#submit-error').html('نوع پرداخت را مشخص کنید.');

            if (cellNumber === '') {
                $('#CellNumber').css('border-color', 'red');
            }
            if (branchId === '') {
                $('#BranchId').css('border-color', 'red');
            }
            if (fullName === '') {
                $('#fullName').css('border-color', 'red');
            }
            if (paymentTypeId === '') {
                $('#PaymentTypeId').css('border-color', 'red');
            }
        }
    } else {
        $('#submit-succes').css('display', 'none');
        $('#submit-error').css('display', 'block');
        $('#submit-error').html('محصولی انتخاب نشده است.');
    }
    unFreezePage();
}

$("#addedAmount").change(function () {
});