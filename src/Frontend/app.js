// Secure Coding Lab - XSS Vulnerability Frontend Code

document.addEventListener('DOMContentLoaded', () => {
    // API endpoints mapping
    const PRODUCTS_API = '/api/products';
    const SEARCH_API = '/api/products/search';
    const LOGIN_API = '/api/auth/login';
    const REGISTER_API = '/api/auth/register';

    // DOM Elements
    const productGrid = document.getElementById('product-grid');
    const productCountBadge = document.getElementById('product-count');
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');
    const searchStatusBanner = document.getElementById('search-status-banner');
    const searchResultsText = document.getElementById('search-results-text');
    
    // Auth DOM Elements
    const btnLoginTrigger = document.getElementById('btn-login-trigger');
    const userProfileBadge = document.getElementById('user-profile-badge');
    const userDisplayName = document.getElementById('user-display-name');
    const btnLogout = document.getElementById('btn-logout');
    
    // Modals
    const productModal = document.getElementById('product-modal');
    const authModal = document.getElementById('auth-modal');
    const productDetailModal = document.getElementById('product-detail-modal');
    
    // Trigger buttons
    const btnAddProduct = document.getElementById('btn-add-product');
    const btnCloseModal = document.getElementById('close-modal-btn');
    const btnCancelModal = document.getElementById('cancel-modal-btn');
    const btnCloseAuth = document.getElementById('close-auth-btn');
    const btnCloseDetail = document.getElementById('close-detail-btn');
    
    // Forms
    const addProductForm = document.getElementById('add-product-form');
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');
    const commentForm = document.getElementById('comment-form');

    // Auth Tabs
    const tabLogin = document.getElementById('tab-login');
    const tabRegister = document.getElementById('tab-register');
    const authModalTitle = document.getElementById('auth-modal-title');

    // State
    let currentUser = JSON.parse(localStorage.getItem('currentUser')) || null;
    let activeProductId = null;

    // Initialize Auth state
    updateAuthUI();

    // Parse URL parameter to check for search query (DOM XSS vector)
    const urlParams = new URLSearchParams(window.location.search);
    const searchQuery = urlParams.get('q');

    // Init page
    if (searchQuery !== null) {
        searchInput.value = searchQuery;
        displaySearchQuery(searchQuery);
        fetchSearchResults(searchQuery);
    } else {
        fetchAllProducts();
    }

    // Parse Hash parameter for Category Filter (DOM XSS category filter vector)
    const hash = window.location.hash;
    if (hash.startsWith('#category=')) {
        const categoryVal = decodeURIComponent(hash.split('=')[1]);
        if (categoryVal) {
            const categoryStatusBanner = document.getElementById('category-status-banner');
            const categoryFilterText = document.getElementById('category-filter-text');
            categoryStatusBanner.classList.remove('hidden');
            // VULNERABLE: Direct insertion of hash parameter into innerHTML (DOM-based XSS)
            categoryFilterText.innerHTML = `Active Category Filter: <strong>${categoryVal}</strong>`;
        }
    }

    // Modal Events - Add Product
    btnAddProduct.addEventListener('click', (e) => {
        e.preventDefault();
        productModal.classList.remove('hidden');
    });

    const closeProductModal = () => {
        productModal.classList.add('hidden');
        addProductForm.reset();
    };

    btnCloseModal.addEventListener('click', closeProductModal);
    btnCancelModal.addEventListener('click', closeProductModal);

    // Modal Events - Auth Modal
    btnLoginTrigger.addEventListener('click', () => {
        authModal.classList.remove('hidden');
    });

    const closeAuthModal = () => {
        authModal.classList.add('hidden');
        loginForm.reset();
        registerForm.reset();
        showLoginTab();
    };

    btnCloseAuth.addEventListener('click', closeAuthModal);

    // Modal Events - Product Detail Modal
    const closeDetailModal = () => {
        productDetailModal.classList.add('hidden');
        commentForm.reset();
        activeProductId = null;
    };

    btnCloseDetail.addEventListener('click', closeDetailModal);
    
    // Close modals when clicking outside
    window.addEventListener('click', (e) => {
        if (e.target === productModal) closeProductModal();
        if (e.target === authModal) closeAuthModal();
        if (e.target === productDetailModal) closeDetailModal();
    });

    // Toggle Tabs on Auth Modal
    tabLogin.addEventListener('click', showLoginTab);
    tabRegister.addEventListener('click', showRegisterTab);

    function showLoginTab() {
        tabLogin.classList.add('active');
        tabRegister.classList.remove('active');
        loginForm.classList.remove('hidden');
        registerForm.classList.add('hidden');
        authModalTitle.innerText = "Sign In";
    }

    function showRegisterTab() {
        tabRegister.classList.add('active');
        tabLogin.classList.remove('active');
        registerForm.classList.remove('hidden');
        loginForm.classList.add('hidden');
        authModalTitle.innerText = "Create Account";
    }

    // Auth Submission Logic
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('login-username').value;
        const password = document.getElementById('login-password').value;

        try {
            const response = await fetch(LOGIN_API, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });
            const result = await response.json();
            if (response.ok && result.success) {
                currentUser = result.data;
                localStorage.setItem('currentUser', JSON.stringify(currentUser));
                updateAuthUI();
                closeAuthModal();
            } else {
                alert('Login failed: ' + (result.message || 'Check username or password'));
            }
        } catch (error) {
            console.error('Login error:', error);
            alert('Failed to contact Authentication service.');
        }
    });

    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('reg-username').value;
        const email = document.getElementById('reg-email').value;
        const password = document.getElementById('reg-password').value;

        try {
            const response = await fetch(REGISTER_API, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, email, password })
            });
            const result = await response.json();
            if (response.ok && result.success) {
                alert('Account registered! Please login.');
                showLoginTab();
            } else {
                alert('Registration failed: ' + (result.message || 'Username or email may exist'));
            }
        } catch (error) {
            console.error('Registration error:', error);
            alert('Failed to contact Authentication service.');
        }
    });

    // Logout logic
    btnLogout.addEventListener('click', () => {
        currentUser = null;
        localStorage.removeItem('currentUser');
        updateAuthUI();
    });

    function updateAuthUI() {
        if (currentUser) {
            btnLoginTrigger.classList.add('hidden');
            userProfileBadge.classList.remove('hidden');
            userDisplayName.innerText = `Hi, ${currentUser.username || currentUser.Username}!`;
            document.getElementById('comment-username').value = currentUser.username || currentUser.Username;
        } else {
            btnLoginTrigger.classList.remove('hidden');
            userProfileBadge.classList.add('hidden');
            userDisplayName.innerText = "Guest";
            document.getElementById('comment-username').value = 'GuestUser';
        }
    }

    // Product Submission logic (Add Product - Stored XSS entrypoint)
    addProductForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const newProduct = {
            name: document.getElementById('p-name').value,
            description: document.getElementById('p-description').value, // description payload
            price: parseFloat(document.getElementById('p-price').value),
            category: document.getElementById('p-category').value,
            stock: parseInt(document.getElementById('p-stock').value),
            imageUrl: document.getElementById('p-image').value || 'https://picsum.photos/300/200' // image XSS breakout payload
        };

        try {
            const response = await fetch(PRODUCTS_API, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newProduct)
            });

            const result = await response.json();
            if (response.ok && result.success) {
                closeProductModal();
                if (searchQuery) {
                    fetchSearchResults(searchQuery);
                } else {
                    fetchAllProducts();
                }
            } else {
                alert('Error creating product: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            console.error('Failed to create product:', error);
            alert('Failed to send product to API.');
        }
    });

    // Comments Submission logic (Add Comment - Stored XSS comment vector)
    commentForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        if (!activeProductId) return;

        const newComment = {
            username: document.getElementById('comment-username').value,
            content: document.getElementById('comment-content').value // payload input
        };

        try {
            const response = await fetch(`${PRODUCTS_API}/${activeProductId}/comments`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newComment)
            });
            const result = await response.json();
            
            if (response.ok && result.success) {
                document.getElementById('comment-content').value = '';
                // Reload product details to show comments
                openProductDetails(activeProductId);
            } else {
                alert('Failed to add comment: ' + (result.message || 'Error'));
            }
        } catch (error) {
            console.error('Add comment error:', error);
            alert('Failed to send comment to API.');
        }
    });

    // VULNERABLE: Displays query parameter directly in HTML without escaping (DOM-based XSS)
    function displaySearchQuery(query) {
        searchStatusBanner.classList.remove('hidden');
        searchResultsText.innerHTML = `Search results for: "<strong>${query}</strong>"`;
    }

    // Fetch all products
    async function fetchAllProducts() {
        try {
            productGrid.innerHTML = '<div class="loading-spinner">Loading product catalog...</div>';
            const response = await fetch(PRODUCTS_API);
            const result = await response.json();
            
            if (result.success && Array.isArray(result.data)) {
                renderProducts(result.data);
            } else {
                productGrid.innerHTML = `<div class="loading-spinner">Failed to load catalog: ${result.message}</div>`;
            }
        } catch (error) {
            console.error('Error fetching products:', error);
            productGrid.innerHTML = '<div class="loading-spinner">Error connecting to Product Service.</div>';
        }
    }

    // Fetch search results
    async function fetchSearchResults(query) {
        try {
            productGrid.innerHTML = '<div class="loading-spinner">Searching catalog...</div>';
            const response = await fetch(`${SEARCH_API}?q=${encodeURIComponent(query)}`);
            const result = await response.json();
            
            if (result.success) {
                let productsList = [];
                if (Array.isArray(result.data)) {
                    productsList = result.data;
                } else if (result.data && typeof result.data === 'object') {
                    productsList = result.data;
                }
                
                if (!Array.isArray(productsList)) {
                    if (productsList.values) {
                        productsList = Array.from(productsList.values);
                    } else if (typeof productsList === 'object') {
                        productsList = [productsList];
                    } else {
                        productsList = [];
                    }
                }
                renderProducts(productsList);
            } else {
                productGrid.innerHTML = `<div class="loading-spinner">Failed to search: ${result.message}</div>`;
            }
        } catch (error) {
            console.error('Error searching products:', error);
            productGrid.innerHTML = '<div class="loading-spinner">Error searching Product Service.</div>';
        }
    }

    // Render products on screen (Vulnerable to Stored XSS)
    function renderProducts(products) {
        productGrid.innerHTML = '';
        
        if (!products || products.length === 0) {
            productGrid.innerHTML = '<div class="loading-spinner">No products found.</div>';
            productCountBadge.innerText = '0 items';
            return;
        }

        productCountBadge.innerText = `${products.length} item(s)`;

        products.forEach(product => {
            const id = product.id || product.Id || (product._id ? product._id.$oid : '');
            const name = product.name || product.Name || 'Unnamed Product';
            const description = product.description || product.Description || 'No description provided.';
            const price = product.price !== undefined ? product.price : (product.Price !== undefined ? product.Price : 0);
            const category = product.category || product.Category || 'General';
            const stock = product.stock !== undefined ? product.stock : (product.Stock !== undefined ? product.Stock : 0);
            const imageUrl = product.imageUrl || product.ImageUrl || 'https://picsum.photos/300/200';

            const card = document.createElement('div');
            card.className = 'product-card';
            card.dataset.id = id;

            // VULNERABLE: Direct description, name, and imageUrl binding using innerHTML (Stored & Attribute-based XSS)
            card.innerHTML = `
                <img class="product-img" src="${imageUrl}" alt="${name}">
                <div class="product-info">
                    <span class="product-category">${category}</span>
                    <h3 class="product-name">${name}</h3>
                    <p class="product-desc">${description}</p>
                    <div class="product-meta">
                        <span class="product-price">$${parseFloat(price).toFixed(2)}</span>
                        <span class="product-stock">Stock: ${stock}</span>
                    </div>
                </div>
            `;

            // Click to open product details
            card.addEventListener('click', () => {
                openProductDetails(id);
            });

            productGrid.appendChild(card);
        });
    }

    // Open single product details with comments (Vulnerable to Stored XSS in comments)
    async function openProductDetails(id) {
        try {
            const response = await fetch(`${PRODUCTS_API}/${id}`);
            const result = await response.json();
            
            if (result.success && result.data) {
                const product = result.data;
                activeProductId = id;
                
                const name = product.name || product.Name || '';
                const description = product.description || product.Description || '';
                const price = product.price !== undefined ? product.price : (product.Price !== undefined ? product.Price : 0);
                const category = product.category || product.Category || '';
                const stock = product.stock !== undefined ? product.stock : (product.Stock !== undefined ? product.Stock : 0);
                const imageUrl = product.imageUrl || product.ImageUrl || 'https://picsum.photos/300/200';
                const comments = product.comments || product.Comments || [];

                // Bind fields
                document.getElementById('detail-img').src = imageUrl;
                
                // VULNERABLE: Binding detail title and category using innerHTML
                document.getElementById('detail-category').innerHTML = category;
                document.getElementById('detail-name').innerHTML = name;
                document.getElementById('detail-price').innerText = `$${parseFloat(price).toFixed(2)}`;
                document.getElementById('detail-stock').innerText = `Stock: ${stock}`;
                
                // VULNERABLE: Binding detail description using innerHTML
                document.getElementById('detail-desc').innerHTML = description;

                // Load Comments
                const commentsList = document.getElementById('comments-list');
                commentsList.innerHTML = '';
                
                if (comments.length === 0) {
                    commentsList.innerHTML = '<div class="text-secondary text-center p-3">No reviews yet. Be the first to review!</div>';
                } else {
                    comments.forEach(comment => {
                        const commentCard = document.createElement('div');
                        commentCard.className = 'comment-card';
                        
                        const commentUser = comment.username || comment.Username || 'Anonymous';
                        const commentContent = comment.content || comment.Content || '';
                        const commentDate = new Date(comment.createdAt || comment.CreatedAt).toLocaleDateString();

                        // VULNERABLE: Render comment user and content using innerHTML (Stored XSS)
                        commentCard.innerHTML = `
                            <div class="comment-header">
                                <span class="comment-author">${commentUser}</span>
                                <span class="comment-date">${commentDate}</span>
                            </div>
                            <div class="comment-body">${commentContent}</div>
                        `;
                        commentsList.appendChild(commentCard);
                    });
                }

                // Show modal
                productDetailModal.classList.remove('hidden');
            } else {
                alert('Failed to load product details.');
            }
        } catch (error) {
            console.error('Error fetching product details:', error);
            alert('Failed to load product details.');
        }
    }
});
