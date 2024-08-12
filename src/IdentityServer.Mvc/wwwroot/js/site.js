(function ($) {
	"use strict"

	// 白屏转圈特效
	$('#preloader').fadeOut('slow', function () {
		$(this).remove();
	});


	// 背景图片指令 data-bg-image
	$("[data-bg-image]").each(function () {
		const img = $(this).data("bg-image");
		$(this).css({
			backgroundImage: "url(" + img + ")"
		});
	});


	window.onload = addNewClass;

	function addNewClass() {
		$('.fxt-template-animation').imagesLoaded().done(function (_) {
			$('.fxt-template-animation').addClass('loaded');
		});
	}

	// 密码可见和取消可见
	$(".toggle-password").on('click', function () {
		$(this).toggleClass("fa-eye fa-eye-slash");
		const input = $($(this).attr("toggle"));
		if (input.attr("type") === "password") {
			input.attr("type", "text");
		} else {
			input.attr("type", "password");
		}
	});
})(jQuery)




