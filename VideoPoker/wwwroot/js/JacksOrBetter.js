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
        var holdLblDetails = $(item).attr('id').replace('hold', '#holdLbl');
        ToggleHold($(this), holdLblDetails);
    });

    $('#cardRow').on('click', '.playerCard', function () {
        var holdLblDetails = $(this).attr('id').replace('card', '#holdLbl');
        ToggleHold($(this), holdLblDetails);
    });

    function ToggleHold(item, holdLblDetails) {
        if ($('#dealButton').hasClass('d-none')) {
            var holdLbl = $(holdLblDetails);
            var card = $(item).parent().find('.playerCard');
            if (holdLbl.hasClass('d-none')) {
                holdLbl.removeClass('d-none');
                card.attr('held', true);
            }
            else {
                holdLbl.addClass('d-none');
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

    $('#betOne').click(function () {
        $.ajax({
            url: '/VideoPoker/IncreaseWagerByOne',
            method: 'POST',
            contentType: 'application/json',
            success: function (data) {
                $('#payTableHolder').html(data);
                UpdateBetAmountUI();
            },
            error: function (xhr, status, error) {
            }
        });
    });

    $('#betMax').click(function () {
        $.ajax({
            url: '/VideoPoker/MaxWager',
            method: 'POST',
            contentType: 'application/json',
            success: function (data) {
                $('#payTableHolder').html(data);
                UpdateBetAmountUI();
            },
            error: function (xhr, status, error) {
            }
        });
    });

    function UpdateBetAmountUI() {
        var betAmount = $('#betAmount');
        console.log($('.activePayCol:first'));
        var newBetAmount = $('.activePayCol:first').attr('creditamount');
        betAmount.attr('wager', newBetAmount);
        betAmount.text('Bet: ' + newBetAmount);
    }
});
