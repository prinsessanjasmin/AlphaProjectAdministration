//This js is created from teacher Hans Mattin-Lasseis video instructions, then I got help from Claude AI to fix some issues 
//because of how my poroject is built. 

function initMemberSelector(config) {
    let activeIndex = -1; 
    let selectedIds = [];
    const selectedIdsInput = document.getElementById(config.selectedIds);

    if (selectedIdsInput && selectedIdsInput.value) {
        try {
            const existingIds = JSON.parse(selectedIdsInput.value);
            if (Array.isArray(existingIds)) {
                selectedIds = existingIds;
            }
        } catch (e) {
            console.error("Error parsing selected IDs", e);
        }
    }

    const memberContainer = document.getElementById(config.containerId);
    const input = document.getElementById(config.inputId);
    const results = document.getElementById(config.resultsId);

    if (!memberContainer || !input || !results || !selectedIdsInput) {
        console.error("Member selector initialization failed: Missing elements", {
            memberContainer, input, results, selectedIds
        });
        return;
    }

    input.addEventListener('focus', () => {
        memberContainer.classList.add('focused');
        results.classList.add('focused');
        if (input.value.trim().length > 0) {
            searchMember(input.value.trim());
        }
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            memberContainer.classList.remove('focused');
            results.classList.remove('focused');
        }, 100);
    });


    input.addEventListener('input', () => {
        const query = input.value.trim();
        activeIndex = -1;

        if (query.length > 0) {
            searchMember(query);

        } else {
            results.style.display = 'none';
            results.innerHTML = '';
        }
    });

    input.addEventListener('keydown', (e) => {
        const items = results.querySelectorAll('.search-item');
        if (items.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                activeIndex = (activeIndex + 1) % items.length;
                updateActiveItem(items);
                break;

            case 'ArrowUp':
                e.preventDefault();
                activeIndex = (activeIndex - 1) % items.length;
                updateActiveItem(items);
                break;

            case 'Enter':
                e.preventDefault();
                if (activeIndex >= 0 && items[activeIndex]) {
                    items[activeIndex].click();
                }
                break;

            case 'Backspace':
                if (input.value === '') {
                    removeLastMember();
                }
                break;
        }
    });

    function updateActiveItem(items) {
        items.forEach(item => item.classList.remove('active'));
        if (items[activeIndex]) {
            items[activeIndex].classList.add('active');
            items[activeIndex].scrollIntoView({ block: 'nearest' });
        }
    }

    function searchMember(query) {
        if (!query) return;

        const url = typeof config.searchUrl === 'function'
            ? config.searchUrl(query)
            : `${config.searchUrl}?term=${encodeURIComponent(query)}`;

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => renderSearchResults(data))
            .catch(error => {
                console.error('Error fetching search results:', error);
                results.innerHTML = `<div class="search-error">Error loading results</div>`
                results.style.display = 'block';
            });
    }

    function renderSearchResults(response) {
        results.innerHTML = ''; 

        const itemsArray = response.data || [];

        if (itemsArray.length > 0) {
            console.log("First item properties:", Object.keys(itemsArray[0]));
            console.log("First item full data:", itemsArray[0]);
            console.log("Looking for property:", config.displayProperty);
        }

        if (itemsArray.length === 0) {
            const noResult = document.createElement('div');
            noResult.classList.add('no-results');
            noResult.textContent = config.emptyMessage || 'No results found';
            results.appendChild(noResult);
        } else {

            itemsArray.forEach(item => {
                const itemId = item.Id || item.id || item.ID;

                if (selectedIds.includes(parseInt(item.id)) ||
                    selectedIds.includes(item.id)) {
                    return;
                }

                const resultItem = document.createElement('div');
                resultItem.classList.add('search-item');
                resultItem.dataset.id = item.Id;

                const displayText = item[config.displayProperty] || item.name || 'Unknown';

                if (config.memberClass === 'member' && item[config.imageProperty]) {

                    const imagePath = item[config.imageProperty].startsWith('http')
                        ? item[config.imageProperty]
                        : (config.avatarFolder || '') + item[config.imageProperty];

                    resultItem.innerHTML =
                        `
                        <img class="user-avatar" src="${imagePath} onerror="this.src='/ProjectImages/Icons/Avatar.svg'"/>
                        <span>${displayText}</span>
                    `;
                } else {
                    resultItem.innerHTML = `<span>${displayText}</span>`;
                }
                   
                resultItem.addEventListener('click', () => addMember(item));
                results.appendChild(resultItem);

            });
        }
        results.style.display = 'block';
        activeIndex = -1;
    }

    function addMember(item) {
        const id = typeof item.Id === 'string' ? item.id : item.id.toString();
        const numericId = parseInt(id);

        if (selectedIds.includes(numericId) || selectedIds.includes(id)) {
            return;
        }

        if (!isNaN(numericId)) {
            selectedIds.push(numericId);
        } else {
            selectedIds.push(id);
        }

        const member = document.createElement('div');
        member.classList.add(config.memberClass || 'member');
        member.dataset.id = id;

        const displayText = item[config.displayProperty] || item.name || 'Unknown';

        if (config.memberClass === 'member' && item[config.imageProperty]) {
            const imagePath = item[config.imageProperty].startsWith('http')
                ? item[config.imageProperty]
                : (config.avatarFolder || '') + item[config.imageProperty];

            member.innerHTML =
                `
                        <img class="user-avatar" src="${imagePath} onerror="this.src='/ProjectImages/Icons/Avatar.svg'"/>
                        <span>${displayText}</span>
                    `;
        } else {
            member.innerHTML = `<span>${displayText}</span>`;
        }

        const removeBtn = document.createElement('span');
        removeBtn.textContent = 'x';
        removeBtn.classList.add('btn-remove');
        removeBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            selectedIds = selectedIds.filter(i => i !== numericId && i !== id);
            member.remove();
            updateSelectedIdsInput();
            
        });

        member.appendChild(removeBtn);
        memberContainer.insertBefore(member, input); 

        input.value = '';
        results.style.display = 'none';

        updateSelectedIdsInput();

        input.focus();
    }

    function removeLastMember() {
        const members = memberContainer.querySelectorAll(`.${config.memberClass || 'member'}`);
        if (members.length > 0) {
            const lastMember = members[members.length - 1];
            const id = lastMember.dataset.id;
            const numericId = parseInt(id);

            selectedIds = selectedIds.filter(i => i !== numericId && i !== id);
            lastMember.remove();
            updateSelectedIdsInput();
        }
    }

    function updateSelectedIdsInput() {

        if (selectedIdsInput) {
            selectedIdsInput.value = JSON.stringify(selectedIds);

            validateField(selectedIdsInput);
        }
    }
}