$(document).ready(function() {

    cryptoId = "90";

    //Assigns the cryptosymbol variable depending which picture the user clicks.

    $("#BitcoinPicture").click(function() {
        cryptoId = "90";
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
    });

    $("#EthereumPicture").click(function() {
        cryptoId = "80";
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
    });

    $("#dogePicture").click(function() {
        cryptoId = "2";
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
    });

    $("#litecoinPicture").click(function() {
        cryptoId = "1";
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
    });


    //Ajax call. Calls Binance API with the selected Crypto Picture symbol.
    $("#Submit").click(function() {
        $.ajax({
            url: 'https://api.coinlore.net/api/ticker/?id=' + cryptoId,
            method: 'GET',
            datatype: 'json',
            success: function(data) {
                if (data) {
                    $("#CryptoLabel").text("Coin Symbol: " + data[0].name);
                    $("#CryptoPrice").text("USD Price: $" + data[0].price_usd);
                }
            }
        });
    });
});