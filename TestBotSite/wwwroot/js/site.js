$('.myradio').click(function () {
    $('.myradio').removeClass('myactive');
    $(this).addClass('myactive');
});

function LiClicked() {
    var btnHidden = (this).children().is("('input[type=button]')")
    if (btnHidden != null) {
        btnHidden.click();
    }
}

$("#saveChangesTest").on("click", function () {
    var form = $(this).parents("form");
    var testId = parseInt($(form).find("#currentTest_ID").val().toString());
    var testName = $(form).find("#currentTest_NameOfTest").val().toString();
    var url = $(form).attr("action");
    $.ajax({
        url: url,
        dataType: "json",
        type: 'POST',
        async: 'async',
        data: { 'testId': testId, 'testName': testName },
        success: function (data) {
            if (data.Redirect) {
                window.location.href = data.Redirect;
            } else if (data.Html) {
                if (data.Html[0]) {
                    alert(data.Html[0])
                }
                if (data.Html[1]) {
                }
            }
        },
        error: function (req, status, e) { }
    });
});

$("#saveChangesQuestion").on("click", function () {
    var form = $(this).parents("form");
    var testId = parseInt($(form).find("#currentTest_ID").val().toString());
    var questionId = parseInt($(form).find("#currentQuestion_ID").val().toString());
    var questionContent = $(form).find("#currentQuestion_Content").val().toString();
    var url = $(form).attr("action");
    $.ajax({
        url: url,
        dataType: "json",
        type: 'POST',
        async: 'async',
        data: { 'testId': testId, 'questionId': questionId, 'questionContent': questionContent },
        success: function (data) {
            if (data.Redirect) {
                window.location.href = data.Redirect;
            } else if (data.Html) {
                if (data.Html[0]) {
                    alert(data.Html[0])
                }
                if (data.Html[1]) {
                }
                if (data.Html[2]) {
                }
            }
        },
        error: function (req, status, e) { }
    });
});