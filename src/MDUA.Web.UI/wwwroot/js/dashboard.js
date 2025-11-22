// Dashboard specific logic
$(document).ready(function () {
    // Refresh Button Animation
    $('.btn-outline-danger').click(function () {
        const btn = $(this);
        const originalText = btn.html();

        btn.html('<span class="spinner-border spinner-border-sm me-1"></span> Loading...');
        btn.prop('disabled', true);

        // Simulate reload - In real app, use AJAX here
        setTimeout(() => {
            btn.html(originalText);
            btn.prop('disabled', false);

            // Optional: Show toast notification
            // showToast("Data refreshed successfully");
        }, 1000);
    });
});