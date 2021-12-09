$(document).ready(function() {

    cryptoSymbol = "BTCUSDT";

    //Assigns the cryptosymbol variable depending which picture the user clicks.
    $("#BitcoinPicture").click(function() {
        cryptoSymbol = "BTCUSDT";
    });

    $("#EthereumPicture").click(function() {
        cryptoSymbol = "ETHUSDT";
    });


    //Ajax call. Calls Binance API with the selected Crypto Picture symbol.
    $("#Submit").click(function() {
        $.ajax({
            url: 'https://api.binance.com/api/v3/ticker/price?symbol=' + cryptoSymbol,
            method: 'GET',
            datatype: 'json',
            success: function(data) {
                if (data) {
                    $("#CryptoLabel").text("Coin Symbol: " + data.symbol);
                    $("#CryptoPrice").text("USD Price: " + data.price);
                }
            }
        });
    });
});