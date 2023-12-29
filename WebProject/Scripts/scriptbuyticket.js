document.addEventListener("DOMContentLoaded", function () {
    let selectedTheater = null;
    let selectedDate = null;
    let selectedTime = null;
    let selectedTickets = {};
    const movieId = document.querySelector('.content-wrapper').dataset.movieId;

    function updateContinueButtonState() {
        const continueButton = document.getElementById('ticket-confirmation-button');
        continueButton.disabled = !(selectedTheater && selectedDate && selectedTime && Object.keys(selectedTickets).length > 0);
    }

    function updateTotalPrice() {
        let total = Object.values(selectedTickets).reduce((sum, ticket) => sum + (ticket.quantity * ticket.price), 0);
        document.getElementById("ticket-total-price").textContent = `Total Price: ${total.toFixed(2)}TL`;
    }

    function clearSelections(selector) {
        document.querySelectorAll(selector).forEach(el => {
            el.style.background = "";
            el.dataset.selected = "false";
        });
    }

    function updateBackgroundAndSelection(container, selector) {
        if (container.dataset.selected === "true") {
            container.style.background = "";
            container.dataset.selected = "false";
        } else {
            clearSelections(selector);
            container.style.background = "rgb(255, 208, 0)";
            container.dataset.selected = "true";
        }
    }

    document.querySelectorAll(".theater-select-container").forEach(container => {
        container.addEventListener("click", function () {
            updateBackgroundAndSelection(this, ".theater-select-container");
            selectedTheater = this.dataset.selected === "true" ? this.dataset.theaterId : null;
            updateContinueButtonState();
        });
    });

    document.querySelectorAll(".date-container").forEach(container => {
        container.addEventListener("click", function () {
            updateBackgroundAndSelection(this, ".date-container");
            selectedDate = this.dataset.selected === "true" ? this.dataset.date : null;
            if (selectedDate) {
                loadTimesForDate(selectedDate);
            } else {
                // If no date is selected, clear the times
                document.getElementById("time-selection").innerHTML = '';
            }
            updateContinueButtonState();
        });
    });

    function loadTimesForDate(date) {
        const timeSelectionContainer = document.getElementById("time-selection");
        timeSelectionContainer.innerHTML = ''; // Clear existing times before loading new ones
        fetch(`/BuyTicket/GetTimesForDate?movieId=${movieId}&date=${date}`)
            .then(response => response.json())
            .then(times => {
                times.forEach(time => {
                    const timeContainer = document.createElement('div');
                    timeContainer.className = 'time-container';
                    timeContainer.dataset.time = time;
                    timeContainer.innerHTML = `<h3>${time}</h3>`;
                    timeContainer.addEventListener('click', function () {
                        updateBackgroundAndSelection(this, ".time-container");
                        selectedTime = this.dataset.selected === "true" ? this.dataset.time : null;
                        updateContinueButtonState();
                    });
                    timeSelectionContainer.appendChild(timeContainer);
                });
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }

    function clearTimeSelection() {
        const timeSelectionContainer = document.getElementById("time-selection");
        timeSelectionContainer.innerHTML = '';
        selectedTime = null;
    }

    document.querySelectorAll(".buy-info-ticket-container").forEach(container => {
        let ticketType = container.dataset.ticketTypeId;
        let priceElement = container.querySelector(".buy-info-ticket-pricing h3");
        if (priceElement) {
            let price = parseFloat(priceElement.textContent.replace('TL', ''));
            container.querySelectorAll(".buy-info-ticket-count-button").forEach(button => {
                button.addEventListener("click", function () {
                    let action = this.dataset.action;
                    selectedTickets[ticketType] = selectedTickets[ticketType] || { quantity: 0, price: price };
                    if (action === 'increase') {
                        selectedTickets[ticketType].quantity++;
                    } else if (action === 'reduce' && selectedTickets[ticketType].quantity > 0) {
                        selectedTickets[ticketType].quantity--;
                    }
                    container.querySelector(".ticket-count-value").textContent = selectedTickets[ticketType].quantity;
                    updateTotalPrice();
                    updateContinueButtonState();
                });
            });
        } else {
            console.error("Price element not found for ticket type: ", ticketType);
        }
    });

    document.getElementById("ticket-confirmation-button").addEventListener("click", submitTicketSelection);
});

function submitTicketSelection() {
    let dataToSend = {
        theaterId: selectedTheater,
        date: selectedDate,
        time: selectedTime,
        tickets: Object.keys(selectedTickets).map(key => ({
            ticketType: key,
            quantity: selectedTickets[key].quantity
        }))
    };

    // Sending the booking confirmation data to the server
    const token = $('[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/BuyTicket/ConfirmTicketSelection',
        type: 'POST',
        headers: {
            "RequestVerificationToken": token
        },
        data: dataToSend,
        success: function (result) {
            if (result.success) {
                window.location.href = result.redirectUrl; // Redirect to the URL provided by the server
            } else {
                alert('Error: ' + result.error); // Display error message from server
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error); // Display a generic error message
        }
    });
}
