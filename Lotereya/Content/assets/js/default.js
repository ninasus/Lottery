var Main = function () {
	var runGoTop = function () {
        $('.gotop').on('click', function (e) {
            $("html, body").animate({
                scrollTop: 0
            }, "slow");
            e.preventDefault();
        });
		$(window).scroll(function () {
			if ($(this).scrollTop() > 200) {
				$('.gotop').fadeIn();
			} else {
				$('.gotop').fadeOut();
			}
		});
	};

    return {
        init: function () {
			runGoTop();
        }
    };
}();


function ContactFormBegin() {
    $(this).addClass('sending');
    $(this).append('<div class="loading"></div>');
    $(this).find('.alert').remove();
}

function ContactFormFailure(data) {
    $(this).removeClass('sending');
    $(this).find('.loading').remove();
    alert(data.responseText);
}

function ContactFormSuccess(data) {
    var that = $(this);
    setTimeout(function () {
        that.removeClass('sending');
        that.find('.loading').remove();
        if (data.success)
            resetForm(that);
        that.append(data.Message);
    }, 700);
}

function resetForm($form) {
    $form.find('input, textarea').val('');
    $form.find('select').prop('selectedIndex', 0);
    $form.find('input:radio, input:checkbox')
         .removeAttr('checked').removeAttr('selected');
}