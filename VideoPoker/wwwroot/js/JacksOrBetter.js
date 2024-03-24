$(document).ready(function () {
    $('#dealButton').click(function () {
        $.ajax({
            url: '/VideoPoker/DealCards',
            method: 'GET',
            success: function (data) {
                $('#cardRow').html(data);
                $('#dealButton').addClass('d-none');
                $('#drawButton').removeClass('d-none');
            },
            error: function (xhr, status, error) {
                console.error('Error fetching partial view:', error);
            }
        });
    });

    $('#drawButton').click(function () {
        var heldCards = GetHeldCards();
        console.log(heldCards);
        $.ajax({
            url: '/VideoPoker/DrawCards',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(heldCards), // Convert data to JSON string
            success: function (data) {
                $('#cardRow').html(data);
                $('#drawButton').addClass('d-none');
                $('#dealButton').removeClass('d-none');
            },
            error: function (xhr, status, error) {
                console.error('Error fetching partial view:', error);
            }
        });
    });


    $('#cardRow').on('click', '.holdBtn', function () {
        ToggleHold($(this));
    });

    $('#cardRow').on('click', '.playerCard', function () {
        ToggleHold($(this));
    });

    function ToggleHold(item) {
        if ($('#dealButton').hasClass('d-none')) {
            var holdLabel = $(item).parent().find('.holdLabel');
            var card = $(item).parent().find('.playerCard');
            if (holdLabel.hasClass('d-none')) {
                holdLabel.removeClass('d-none');
                card.attr('held', true);
            }
            else {
                holdLabel.addClass('d-none');
                card.removeAttr('held', true);
            }
        }
    }

    function GetHeldCards() {
        var heldCards = {};
        $('.playerCard').each(function (index, card) {
            var cardDetails = null;
            if ($(card).attr('held') === 'true') {
                cardDetails = {
                    Suit: parseInt($(card).attr('suit')),
                    Rank: parseInt($(card).attr('rank'))
                };
            }
            switch (index) {
                case 0:
                    heldCards.Card1 = cardDetails
                    break;
                case 1:
                    heldCards.Card2 = cardDetails
                    break;
                case 2:
                    heldCards.Card3 = cardDetails
                    break;
                case 3:
                    heldCards.Card4 = cardDetails
                    break;
                case 4:
                    heldCards.Card5 = cardDetails
                    break;
            }
        });
        return heldCards;
    }
});