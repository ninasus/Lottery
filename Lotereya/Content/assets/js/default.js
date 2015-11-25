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