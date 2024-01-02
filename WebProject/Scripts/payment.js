document.addEventListener("DOMContentLoaded", function () {

    let cardNumberInput = document.getElementById("cardnumber");
    let cardMonthInput = document.getElementById("month");
    let cardYearInput = document.getElementById("year");
    let cardCvvInput = document.getElementById("cvv");
    let cardNameInput = document.getElementById("cardname");

    let cardNumberPreview = document.getElementById("card-front-text1");
    let cardDatePreview = document.getElementById("card-front-text2");
    let cardNamePreview = document.getElementById("card-front-text3");
    let cardCvvPreview = document.getElementById("card-back-text");

    let confirmPaymentButton = document.querySelector('.btn-confirm-payment');
    confirmPaymentButton.disabled = true; // Disable button initially

    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth() + 1; // January is 0

    // Function to check if all inputs are valid
    function areAllInputsValid() {
        const cardNumberValid = cardNumberInput.value.length === 16;
        const cardMonthValid = parseInt(cardMonthInput.value) >= 1 && parseInt(cardMonthInput.value) <= 12;
        const cardYearValid = parseInt(cardYearInput.value) >= currentYear;
        const cardCvvValid = cardCvvInput.value.length === 3;
        const cardNameValid = cardNameInput.value.trim().length > 0;
        const expiryDateValid = parseInt(cardYearInput.value) > currentYear || (parseInt(cardYearInput.value) === currentYear && parseInt(cardMonthInput.value) >= currentMonth);

        return cardNumberValid && cardMonthValid && cardYearValid && cardCvvValid && cardNameValid && expiryDateValid;
    }

    // Function to show error messages
    function showErrorMessage() {
        let errorMessage = '';

        if (cardNumberInput.value.length !== 16) {
            errorMessage += 'Card number must be 16 digits long.\n';
        }
        if (!(parseInt(cardMonthInput.value) >= 1 && parseInt(cardMonthInput.value) <= 12)) {
            errorMessage += 'Invalid month in expiry date.\n';
        }
        if (!(parseInt(cardYearInput.value) >= currentYear)) {
            errorMessage += 'Invalid year in expiry date.\n';
        }
        if (cardCvvInput.value.length !== 3) {
            errorMessage += 'CVV must be 3 digits long.\n';
        }

        if (errorMessage) {
            alert(errorMessage);
            return false;
        }
        return true;
    }

    // Event listener to validate input fields
    document.querySelectorAll('.payment-form input').forEach(input => {
        input.addEventListener('input', () => {
            confirmPaymentButton.disabled = !areAllInputsValid();
        });
    });

    // Confirm Payment button click event
    confirmPaymentButton.addEventListener('click', function (event) {
        event.preventDefault();
        if (areAllInputsValid()) {
            if (showErrorMessage()) {
                if (window.confirm("Do you want to proceed with the transaction?")) {
                    // AJAX request to confirm the payment
                    fetch('/BuyTicket/ConfirmPayment', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                    }).then(response => {
                        if (response.ok) {
                            return response.json();
                        } else {
                            throw new Error('Network response was not ok.');
                        }
                    }).then(data => {
                        if (data.success) {
                            window.location.href = '/BuyTicket/ShowTicket'; // Update with the correct URL
                        } else {
                            alert('Transaction failed. Please try again.');
                        }
                    }).catch(error => {
                        console.error('Error:', error);
                        alert('An error occurred during the transaction.');
                    });
                }
            }
        }
    });

    // Function to update card number preview
    cardNumberInput.addEventListener("input", function () {
        cardNumberPreview.textContent = this.value;
    });

    // Function to update card date preview
    cardMonthInput.addEventListener("input", updateDatePreview);
    cardYearInput.addEventListener("input", updateDatePreview);

    function updateDatePreview() {
        let month = cardMonthInput.value.padStart(2, "0"); // Ensure two digits
        let year = cardYearInput.value.slice(-2); // Take last two digits of year
        cardDatePreview.textContent = `${month}/${year}`;
    }

    // Function to update card CVV preview
    cardCvvInput.addEventListener("input", function () {
        cardCvvPreview.textContent = this.value;
    });

    // Function to update card name preview
    cardNameInput.addEventListener("input", function () {
        cardNamePreview.textContent = this.value;
    });
    cardNumberInput.addEventListener("input", function (e) {
        // Allow only digits
        let value = this.value.replace(/\D/g, "");
        // Limit length to 16 digits
        this.value = value.slice(0, 16);
    });

    let countdownDisplay = document.getElementById('countdown-timer');
    let countdownTime = 5 * 60 * 1000; // 5 minutes in milliseconds
    let alertShown = false;

    // Function to update countdown display
    function updateCountdownDisplay(timeLeft) {
        let minutes = Math.floor(timeLeft / 60000);
        let seconds = ((timeLeft % 60000) / 1000).toFixed(0);
        countdownDisplay.textContent = minutes + ":" + (seconds < 10 ? '0' : '') + seconds;
    }

    // Set a timer for 5 minutes
    let timer = setInterval(function () {
        countdownTime -= 1000;
        updateCountdownDisplay(countdownTime);

        if (countdownTime <= 0) {
            clearInterval(timer);

            if (!alertShown) {
                alert("Time's up! Redirecting to another page.");
                alertShown = true;
                navigator.sendBeacon('/BuyTicket/ClearPendingReservations');
                window.location.href = '/Movies/OnTheaters';
            }
        }
    }, 1000);

    window.addEventListener("beforeunload", function () {
        // Call your method to clean up pending reservations
        navigator.sendBeacon('/BuyTicket/ClearPendingReservations');
    });
});