$(document).ready(function () {
    const $options = $(".crypto-option");
    const defaultId = "90";
    const currencyFormatter = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 2
    });
    const wholeDollarFormatter = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 0
    });

    let selectedCrypto = getSelectionFromOption($options.filter(".selected").first()) ?? {
        id: defaultId,
        name: "Bitcoin",
        symbol: "BTC"
    };

    function getSelectionFromOption($option) {
        if (!$option.length) {
            return null;
        }

        return {
            id: String($option.data("crypto-id")),
            name: $option.data("crypto-name"),
            symbol: $option.data("crypto-symbol")
        };
    }

    function setStatus(message, styleClass) {
        const $status = $("#CryptoStatus");

        $status
            .removeClass("alert-secondary alert-success alert-danger alert-warning alert-info")
            .addClass(styleClass)
            .text(message);
    }

    function setLoading(isLoading) {
        $("#Submit, #SubmitInline, #UseCustomCrypto, #ResetSelection, #ResetSelectionInline").prop("disabled", isLoading);
        $("#Submit, #SubmitInline").text(isLoading ? "Refreshing..." : "Refresh price");
    }

    function formatPrice(value) {
        const numericValue = Number(value);

        if (!Number.isFinite(numericValue)) {
            return "N/A";
        }

        if (numericValue >= 1) {
            return currencyFormatter.format(numericValue);
        }

        return `$${numericValue.toFixed(6)}`;
    }

    function formatWholeDollars(value) {
        const numericValue = Number(value);

        if (!Number.isFinite(numericValue)) {
            return "N/A";
        }

        return wholeDollarFormatter.format(numericValue);
    }

    function formatPercentChange(value) {
        const numericValue = Number(value);

        if (!Number.isFinite(numericValue)) {
            return "--";
        }

        return `${numericValue > 0 ? "+" : ""}${numericValue.toFixed(2)}%`;
    }

    function syncSelectedCard() {
        $options.removeClass("selected").attr("aria-pressed", "false");

        const $matchingOption = $options.filter(`[data-crypto-id='${selectedCrypto.id}']`).first();

        if ($matchingOption.length) {
            $matchingOption.addClass("selected").attr("aria-pressed", "true");
        }

        $("#SelectedCryptoName, #SelectedCryptoNameInline").text(selectedCrypto.name);
        $("#SelectedCryptoSymbol, #SelectedCryptoSymbolInline").text(`${selectedCrypto.symbol} · Coinlore id ${selectedCrypto.id}`);
    }

    function updateMetricStyles(changeValue) {
        const numericValue = Number(changeValue);
        const $changeMetric = $("#CryptoChange24h");
        const $changeInlineBadge = $("#CryptoChange24hInlineBadge");

        $changeMetric.removeClass("crypto-positive crypto-negative crypto-neutral");
        $changeInlineBadge.removeClass("text-success-emphasis text-danger-emphasis text-body-emphasis border-success-subtle border-danger-subtle border-light-subtle bg-success-subtle bg-danger-subtle bg-body-secondary");

        if (!Number.isFinite(numericValue) || numericValue === 0) {
            $changeMetric.addClass("crypto-neutral");
            $changeInlineBadge.addClass("bg-body-secondary text-body-emphasis border-light-subtle");
            return;
        }

        $changeMetric.addClass(numericValue > 0 ? "crypto-positive" : "crypto-negative");
        $changeInlineBadge.addClass(numericValue > 0
            ? "bg-success-subtle text-success-emphasis border-success-subtle"
            : "bg-danger-subtle text-danger-emphasis border-danger-subtle");
    }

    function updateDashboard(coin) {
        selectedCrypto = {
            id: String(coin.id ?? selectedCrypto.id),
            name: coin.name ?? selectedCrypto.name,
            symbol: coin.symbol ?? selectedCrypto.symbol
        };

        syncSelectedCard();

        $("#CryptoLabel").text(selectedCrypto.name);
        $("#CryptoTicker").text(selectedCrypto.symbol);
        $("#CryptoPrice, #CryptoPriceInline").text(formatPrice(coin.price_usd));
        $("#CryptoChange24h, #CryptoChange24hInline").text(formatPercentChange(coin.percent_change_24h));
        $("#CryptoMarketCap").text(formatWholeDollars(coin.market_cap_usd));
        $("#CryptoRank").text(coin.rank ? `#${coin.rank}` : "--");

        updateMetricStyles(coin.percent_change_24h);
    }

    function fetchCrypto(id) {
        setLoading(true);
        setStatus(`Loading live data for ${selectedCrypto.name}...`, "alert-info");

        $.ajax({
            url: "https://api.coinlore.net/api/ticker/?id=" + encodeURIComponent(id),
            method: "GET",
            dataType: "json",
            success: function (data) {
                if (!Array.isArray(data) || data.length === 0) {
                    setStatus("No market data was returned for that Coinlore id.", "alert-warning");
                    return;
                }

                updateDashboard(data[0]);
                setStatus(`Updated ${selectedCrypto.name} using the live Coinlore API.`, "alert-success");
            },
            error: function () {
                setStatus("Could not reach the Coinlore API. Please try again in a moment.", "alert-danger");
            },
            complete: function () {
                setLoading(false);
            }
        });
    }

    $options.on("click", function () {
        const optionSelection = getSelectionFromOption($(this));

        if (!optionSelection) {
            return;
        }

        selectedCrypto = optionSelection;
        syncSelectedCard();
        fetchCrypto(selectedCrypto.id);
    });

    $("#Submit, #SubmitInline").on("click", function () {
        fetchCrypto(selectedCrypto.id);
    });

    $("#ResetSelection, #ResetSelectionInline").on("click", function () {
        selectedCrypto = {
            id: defaultId,
            name: "Bitcoin",
            symbol: "BTC"
        };

        syncSelectedCard();
        fetchCrypto(selectedCrypto.id);
    });

    $("#UseCustomCrypto").on("click", function () {
        const customId = $("#CustomCryptoId").val()?.toString().trim();

        if (!customId) {
            setStatus("Enter a Coinlore id to load a custom coin.", "alert-warning");
            return;
        }

        selectedCrypto = {
            id: customId,
            name: "Custom coin",
            symbol: "Custom"
        };

        syncSelectedCard();
        fetchCrypto(customId);
    });

    $("#CustomCryptoId").on("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            $("#UseCustomCrypto").trigger("click");
        }
    });

    syncSelectedCard();
    fetchCrypto(selectedCrypto.id);
});
