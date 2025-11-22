$(document).ready(function () {
    const $container = $('#appContainer');
    const $toggleBtn = $('#sidebarToggle');
    const STORE_KEY = 'sidebar-state';

    // 1. Check LocalStorage on Load
    // If user is on desktop and preferred state is 'closed', hide it immediately
    if (window.innerWidth >= 992) {
        const savedState = localStorage.getItem(STORE_KEY);
        if (savedState === 'hidden') {
            $container.addClass('toggled');
        }
    }

    // 2. Toggle Button Click
    $toggleBtn.on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $container.toggleClass('toggled');

        // Save state (only relevant for desktop)
        if (window.innerWidth >= 992) {
            if ($container.hasClass('toggled')) {
                localStorage.setItem(STORE_KEY, 'hidden');
            } else {
                localStorage.setItem(STORE_KEY, 'visible');
            }
        }
    });

    // 3. Close sidebar when clicking outside (Mobile Only)
    $(document).on('click', function (e) {
        if (window.innerWidth < 992) {
            if ($container.hasClass('toggled') &&
                !$(e.target).closest('.sidebar-wrapper').length &&
                !$(e.target).closest('#sidebarToggle').length) {

                $container.removeClass('toggled');
            }
        }
    });
});