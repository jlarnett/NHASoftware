const input = document.getElementById('searchInput');
const dropdown = document.getElementById('searchDropdown');
let debounceTimeout;

input.addEventListener('input', () => {
    clearTimeout(debounceTimeout);

    debounceTimeout = setTimeout(async () => {
    const searchString = input.value.trim();

    if (searchString.length > 0) {
        try {
        const response = await fetch(`/api/search/${encodeURIComponent(searchString)}`);

        if (!response.ok) {
            console.error('Search API error:', response.statusText);
            dropdown.style.display = 'none';
            return;
        }

        const data = await response.json();

        // Clear previous results
        dropdown.innerHTML = '';

        // Combine anime and game results
        const animeResults = data.animePages || [];
        const gameResults = data.gamePages || [];
        const userResults = data.users || [];

            // If no results, hide dropdown
        if (animeResults.length === 0 && gameResults.length === 0 && userResults.length === 0) {
            dropdown.style.display = 'none';
            return;
        }

        // Example:
        const searchRegex = new RegExp(`(${searchString})`, 'gi'); // 'g' = global, 'i' = case-insensitive

        // Create list items for anime results
        animeResults.forEach(item => {
            const li = document.createElement('li');
            const highlightedName = item.animeName.replace(searchRegex, '<strong>$1</strong>');
            li.innerHTML = `<img class='col-1 me-2' src='${item.animeImageUrl}'></img><strong>Anime:</strong> ${highlightedName}`;
            li.style.padding = '8px';
            li.style.cursor = 'pointer';

            // Build URL dynamically using current origin
            const animeUrl = `${window.location.origin}/Anime/AnimePage/${item.id || item.animePageId}`;

            li.addEventListener('click', () => {
            window.location.href = animeUrl;
            });

            dropdown.appendChild(li);
        });

        // Create list items for game results
        gameResults.forEach(item => {
            const li = document.createElement('li');
            const highlightedName = item.name.replace(searchRegex, '<strong>$1</strong>');
            li.innerHTML = `<img class='col-1 me-2' src='${item.imageUrl}'></img><strong>Game:</strong> ${highlightedName}`;
            li.style.padding = '8px';
            li.style.cursor = 'pointer';

            // Build URL dynamically using current origin
            const gameUrl = `${window.location.origin}/Game/GamePage/${item.id || item.gamePageId}`;

            li.addEventListener('click', () => {
            window.location.href = gameUrl;
            });

            dropdown.appendChild(li);
        });

        // Create list items for game results
        userResults.forEach(item => {
            console.log(item);
            const li = document.createElement('li');
            const highlightedName = item.displayName.replace(searchRegex, '<strong>$1</strong>');
            li.innerHTML = `<img class='col-1 me-2' src='/ProfilePictures/${item.profilePicturePath}'></img><strong>User:</strong> ${highlightedName} ${item.email}`;
            li.style.padding = '8px';
            li.style.cursor = 'pointer';

            // Build URL dynamically using current origin
            const profileUrl = `${window.location.origin}/Users/GetProfiles?UserId=${item.id}`;

            li.addEventListener('click', () => {
            window.location.href = profileUrl;
            });

            dropdown.appendChild(li);
        });

        dropdown.style.display = 'block';
        } catch (error) {
        console.error('Fetch error:', error);
        dropdown.style.display = 'none';
        }
    } else {
        dropdown.style.display = 'none';
    }
    }, 500);
});

// Optional: hide dropdown if user clicks outside
document.addEventListener('click', (e) => {
    if (!input.contains(e.target) && !dropdown.contains(e.target)) {
    dropdown.style.display = 'none';
    }
});