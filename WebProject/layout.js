document.addEventListener("DOMContentLoaded", function () {
    const searchIcon = document.getElementById("searchIcon");
    const searchInput = document.getElementById("searchInput");
    const loginButton = document.getElementById("login");
    const loginContainer = document.getElementById("logincontainer");
    const closeButton = document.getElementById("closebutton");
    const topButton = document.getElementById("top-button");
    const signupButton = document.getElementById("sign-up-button");
    const loginForm = document.getElementById("login-form");
    const registerForm = document.getElementById("register-form");
    const profileButton = document.getElementById("profile");
    const profilename = document.getElementById("login-username");
    const logoutButton = document.getElementById("logout");
    // Search Icon and Input Event Listeners
    if (searchIcon && searchInput) {
        searchIcon.addEventListener("mouseenter", function () {
            searchInput.classList.add("visible");
        });

        searchIcon.addEventListener("mouseleave", function () {
            setTimeout(function () {
                if (!searchInput.matches(":focus")) {
                    searchInput.classList.remove("visible");
                }
            }, 3000);
        });

        searchInput.addEventListener("blur", function () {
            searchInput.classList.remove("visible");
        });
    }

    // Login Button Event Listener
    if (loginButton && loginContainer && loginForm && registerForm) {
        loginButton.addEventListener("click", function () {
            loginContainer.style.visibility = "visible";
            loginContainer.style.opacity = "1";
            document.body.classList.add("body-blur");
            loginForm.style.display = "block";
            registerForm.style.display = "none";
        });
    }

    // Close Button Event Listener
    if (closeButton) {
        closeButton.addEventListener("click", function () {
            loginContainer.style.visibility = "hidden";
            loginContainer.style.opacity = "0";
            document.body.classList.remove("body-blur");
        });
    }

    // Return to Top Button Event Listener
    if (topButton) {
        window.addEventListener("scroll", function () {
            if (window.scrollY > 320) {
                topButton.classList.add("show");
            } else {
                topButton.classList.remove("show");
            }
        });

        topButton.addEventListener("click", function () {
            window.scrollTo({
                top: 0,
                behavior: "smooth",
            });

            topButton.classList.add("active");

            setTimeout(function () {
                topButton.classList.remove("active");
            }, 300);
        });
    }





    $(document).ready(function () {
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();
        var url = $(this).data('login-url');
        var token = $('[name=__RequestVerificationToken]').val();
        var username = $('#username').val();
        var password = $('#password').val();

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                __RequestVerificationToken: token,
                username: username,
                password: password
            },
            success: function (result) {
                if (result.success) {
                    loginContainer.style.visibility = "hidden";
                    loginContainer.style.opacity = "0";
                    document.body.classList.remove("body-blur");
                    $('#login').hide();
                    $('#profile').show().find('#login-username').text(result.username);
                    $('#logout').show();
                    setTimeout(function () {
                        location.reload();
                    }, 500);
                } else {
                    $('#loginError').text(result.error).show();
                }
            },
            error: function (xhr, status, error) {
                $('#loginError').text('An error occurred.').show();
            }
        });
    });
        $('#sign-up-button').on('click', function (e) {
            e.preventDefault();
            $('#login-form').hide();
            $('#register-form').show();
        });
        $('#registerForm').on('submit', function (e) {
            e.preventDefault();
            console.log("Register form submitted");

            var url = $(this).data('register-url');
            var token = $('[name="__RequestVerificationToken"]', this).val();
            var name = $('#reg-name').val();
            var surname = $('#reg-surname').val();
            var username = $('#reg-username').val();
            var email = $('#reg-email').val();
            var password = $('#reg-password').val();

            console.log("URL: ", url); // Debugging

            $.ajax({
                url: url,
                type: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    name: name,
                    surname: surname,
                    username: username,
                    email: email,
                    password: password
                },
                success: function (result) {
                    console.log("Success response: ", result); // Debugging
                    if (result.success) {
                        $('#register-form').hide();
                        $('#login-container-message').show();
                        setTimeout(function () {
                            location.reload();
                        }, 3000);
                    } else {
                        $('#signupError').text(result.error).show();
                    }
                },
                error: function (xhr, status, error) {
                    console.log("Error response: ", xhr, status, error); // Debugging
                    $('#signupError').text('An error occurred during registration.').show();
                }
            });
        });
    });

});

