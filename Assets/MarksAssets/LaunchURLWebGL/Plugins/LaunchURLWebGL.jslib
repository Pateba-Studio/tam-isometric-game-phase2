mergeInto(LibraryManager.library, {
	LaunchURLWebGL_launchURL: function (url, windowName, windowFeatures) {
		url            = UTF8ToString(url);
		windowName     = UTF8ToString(windowName);
		windowFeatures = UTF8ToString(windowFeatures);
		document.documentElement.addEventListener('pointerup', function () {
			window.open(url, windowName, windowFeatures);
		}, { once: true });
    }
});