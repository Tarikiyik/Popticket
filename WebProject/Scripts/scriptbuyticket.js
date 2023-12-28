document.addEventListener("DOMContentLoaded", function () {
    let selectedTheater = null;
    let selectedDate = null;
    let selectedTime = null;
    let selectedTickets = {};

    // Function to clear selections
    function clearSelections(selector) {
        document.querySelectorAll(selector).forEach(el => el.classList.remove('selected'));
    }

    // Function to update total price
    function updateTotalPrice() {
        let total = Object.values(selectedTickets).reduce((sum, ticket) => {
            return sum + (ticket.quantity * ticket.price);
        }, 0);
        document.getElementById("ticket-total-price").textContent = `Total Price: ${total.toFixed(2)}TL`;
    }

    // Function to enable/disable the continue button
    function updateContinueButtonState() {
        const continueButton = document.getElementById('ticket-confirmation-button');
        continueButton.disabled = !(selectedTheater && selectedDate && selectedTime && Object.keys(selectedTickets).length > 0);
    }

    // Event listeners for theater selection
    document.querySelectorAll(".theater-select-container").forEach(container => {
        container.addEventListener("click", function () {
            clearSelections(".theater-select-container.selected");
            this.classList.add('selected');
            selectedTheater = this.dataset.theaterId;
            updateContinueButtonState();
        });
    });

    // Event listeners for date selection
    document.querySelectorAll("#date-selection .date-container").forEach(container => {
        container.addEventListener("click", function () {
            clearSelections("#date-selection .date-container.selected");
            this.classList.add('selected');
            selectedDate = this.dataset.date;
            // TODO: Load the times for the selected date here if needed
            updateContinueButtonState();
        });
    });

    // Event listeners for time selection
    document.querySelectorAll("#time-selection .time-container").forEach(container => {
        container.addEventListener("click", function () {
            clearSelections("#time-selection .time-container.selected");
            this.classList.add('selected');
            selectedTime = this.dataset.time;
            updateContinueButtonState();
        });
    });

    // Event listeners for ticket count buttons
    document.querySelectorAll(".buy-info-ticket-container").forEach(container => {
        let ticketType = container.dataset.ticketTypeId;
        let price = parseFloat(container.querySelector(".buy-info-ticket-pricing h3").textContent.replace('TL', ''));
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
    });

    // Event listener for the continue button
    document.getElementById("ticket-confirmation-button").addEventListener("click", function () {
        let dataToSend = {
            theaterId: selectedTheater,
            date: selectedDate,
            time: selectedTime,
            tickets: Object.keys(selectedTickets).map(key => ({
                ticketType: key,
                quantity: selectedTickets[key].quantity
            }))
        };

        // AJAX request to server
        $.ajax({
            url: '/BuyTicket/ConfirmTicketSelection', // Replace with your actual endpoint
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),
            dataType: 'json',
            success: function (response) {
                // On success, redirect to the next step using returned URL
                window.location.href = response.redirectUrl;
            },
            error: function (xhr, status, error) {
                // Handle errors here
                console.error("Error occurred: ", status, error);
            }
        });
    });
});
