$(document).ready(function () {
    var searchInput = $('#searchInput'); // The ID of the search input field on the SearchResults page

    // Trigger AJAX search when search icon is clicked or Enter key is pressed
    searchInput.on('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            performAjaxSearch();
        }
    });

    $('#searchIcon').on('click', function () {
        performAjaxSearch();
    });

    function performAjaxSearch() {
        var query = searchInput.val().trim();
        if (query) {
            $.ajax({
                url: '/Search/AjaxSearchResults',
                type: 'GET',
                dataType: 'json',
                data: { query: query },
                success: function (movies) {
                    buildSearchResults(movies);
                },
                error: function () {
                    alert('Error occurred while searching');
                }
            });
        }
    }

    function buildSearchResults(movies) {
        var resultsContainer = $('.search-result');
        resultsContainer.empty(); // Clear previous results

        if (movies.length > 0) {
            $.each(movies, function (i, movie) {
                var item = $('<div class="search-item">').append(
                    $('<img>').addClass('search-banner').attr('src', movie.ImagePath),
                    $('<div>').addClass('search-item-details').append(
                        $('<h3>').addClass('search-header').text(movie.Title),
                        $('<h3>').append($('<img>').attr('src', './images/imdb.png'), movie.IMDBRating),
                        $('<h3>').append($('<img>').attr('src', './images/time.png'), movie.Runtime)
                    )
                );
                resultsContainer.append(item);
            });
        } else {
            resultsContainer.append($('<p>').text('No results found.'));
        }
    }

});