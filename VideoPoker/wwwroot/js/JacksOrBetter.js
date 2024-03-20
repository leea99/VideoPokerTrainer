$(document).ready(function () {
    $('#dealButton').click(function () {
        // Make an AJAX request to fetch the partial view
        $.ajax({
            url: '/VideoPoker/DealCards',
            method: 'GET',
            success: function (data) {
                // Replace the content of the row with the fetched partial view
                $('#cardsRow').html(data);
            },
            error: function (xhr, status, error) {
                console.error('Error fetching partial view:', error);
            }
        });
    });
});