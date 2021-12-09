$(document).ready(function() {
    $("#Submit").click(function() {
        $.ajax({
            url: 'https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT',
            method: 'GET',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function(data) {
                if (data.success) {
                    $("#CryptoLabel").text(data.symbol);
                    $("#CryptoPrice").text(data.price);
                }
            }
        });
    });
});