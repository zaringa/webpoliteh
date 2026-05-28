const PRODUCTS = [
  {
    id: 'p-101',
    name: 'Наушники Orbit Pro',
    category: 'Аудио',
    price: 7490,
  },
  {
    id: 'p-102',
    name: 'Клавиатура Flow TKL',
    category: 'Периферия',
    price: 5690,
  },
  {
    id: 'p-103',
    name: 'Монитор Nova 27',
    category: 'Мониторы',
    price: 21990,
  },
  {
    id: 'p-104',
    name: 'SSD FlashCore 1TB',
    category: 'Накопители',
    price: 8890,
  },
];

const productById = new Map(PRODUCTS.map((product) => [product.id, product]));
const moneyFormatter = new Intl.NumberFormat('ru-RU', {
  style: 'currency',
  currency: 'RUB',
  maximumFractionDigits: 0,
});

const elements = {
  productsGrid: document.getElementById('products-grid'),
  catalogSummary: document.getElementById('catalog-summary'),
  cartToggleBtn: document.getElementById('cart-toggle-btn'),
  closeCartBtn: document.getElementById('close-cart-btn'),
  cartCountBadge: document.getElementById('cart-count-badge'),
  cartOverlay: document.getElementById('cart-overlay'),
  cartPanel: document.getElementById('cart-panel'),
  cartItemsList: document.getElementById('cart-items-list'),
  emptyCartState: document.getElementById('empty-cart-state'),
  cartSubtitle: document.getElementById('cart-subtitle'),
  subtotalCount: document.getElementById('subtotal-count'),
  subtotalValue: document.getElementById('subtotal-value'),
  clearCartBtn: document.getElementById('clear-cart-btn'),
};

const state = {
  cart: {},
  isCartOpen: false,
};

function formatPrice(value) {
  return moneyFormatter.format(value);
}

function pluralizeItems(count) {
  if (count % 10 === 1 && count % 100 !== 11) {
    return `${count} товар`;
  }

  if ([2, 3, 4].includes(count % 10) && ![12, 13, 14].includes(count % 100)) {
    return `${count} товара`;
  }

  return `${count} товаров`;
}

function getCartItems() {
  return Object.entries(state.cart)
    .map(([productId, quantity]) => {
      const product = productById.get(productId);
      if (!product) {
        return null;
      }

      return {
        ...product,
        quantity,
        lineTotal: quantity * product.price,
      };
    })
    .filter(Boolean);
}

function getTotals() {
  return getCartItems().reduce(
    (acc, item) => {
      return {
        itemsCount: acc.itemsCount + item.quantity,
        totalPrice: acc.totalPrice + item.lineTotal,
      };
    },
    { itemsCount: 0, totalPrice: 0 },
  );
}

function addToCart(productId) {
  if (!productById.has(productId)) {
    return;
  }

  state.cart[productId] = (state.cart[productId] ?? 0) + 1;
  render(true);
}

function clearCart() {
  if (Object.keys(state.cart).length === 0) {
    return;
  }

  state.cart = {};
  render(true);
}

function toggleCart() {
  state.isCartOpen = !state.isCartOpen;
  render(false);
}

function closeCart() {
  if (!state.isCartOpen) {
    return;
  }

  state.isCartOpen = false;
  render(false);
}

function renderProducts() {
  elements.productsGrid.innerHTML = PRODUCTS.map((product) => {
    const quantity = state.cart[product.id] ?? 0;

    return `
      <li class="rounded-2xl border border-slate-800 bg-slate-900/70 p-4 transition hover:-translate-y-0.5 hover:border-cyan-400/40">
        <div class="flex items-start justify-between gap-3">
          <p class="rounded-full border border-slate-700 bg-slate-800 px-2.5 py-1 text-xs font-semibold text-slate-300">${product.category}</p>
          <span class="text-xs ${quantity > 0 ? 'text-emerald-300' : 'text-slate-400'}">
            ${quantity > 0 ? `В корзине: ${quantity}` : 'Еще не добавлен'}
          </span>
        </div>

        <h3 class="mt-4 text-lg font-bold">${product.name}</h3>

        <div class="mt-5 flex items-center justify-between gap-3">
          <p class="text-base font-extrabold text-cyan-300">${formatPrice(product.price)}</p>
          <button
            type="button"
            data-action="add"
            data-product-id="${product.id}"
            class="rounded-full bg-cyan-400 px-4 py-2 text-xs font-extrabold uppercase tracking-[0.08em] text-slate-950 transition hover:bg-cyan-300"
          >
            В корзину
          </button>
        </div>
      </li>
    `;
  }).join('');
}

function renderCartItems() {
  const cartItems = getCartItems();

  if (cartItems.length === 0) {
    elements.emptyCartState.classList.remove('hidden');
    elements.cartItemsList.classList.add('hidden');
    elements.cartItemsList.innerHTML = '';
    elements.clearCartBtn.disabled = true;
    return;
  }

  elements.emptyCartState.classList.add('hidden');
  elements.cartItemsList.classList.remove('hidden');
  elements.clearCartBtn.disabled = false;

  elements.cartItemsList.innerHTML = cartItems
    .map((item) => {
      return `
        <li class="rounded-2xl border border-slate-800 bg-slate-800/70 p-4">
          <div class="flex items-center justify-between gap-3">
            <h3 class="text-sm font-bold text-slate-100">${item.name}</h3>
            <span class="rounded-full border border-slate-700 px-2.5 py-1 text-xs font-semibold text-slate-200">x${item.quantity}</span>
          </div>

          <div class="mt-3 flex items-center justify-between gap-3 text-sm">
            <p class="text-slate-300">${formatPrice(item.price)} за шт.</p>
            <p class="font-extrabold text-cyan-300">${formatPrice(item.lineTotal)}</p>
          </div>
        </li>
      `;
    })
    .join('');
}

function renderTotals() {
  const totals = getTotals();

  elements.cartCountBadge.textContent = String(totals.itemsCount);
  elements.catalogSummary.textContent = `В корзине: ${totals.itemsCount} шт.`;
  elements.cartSubtitle.textContent = pluralizeItems(totals.itemsCount);
  elements.subtotalCount.textContent = pluralizeItems(totals.itemsCount);
  elements.subtotalValue.textContent = formatPrice(totals.totalPrice);
}

function renderCartVisibility() {
  if (state.isCartOpen) {
    elements.cartOverlay.classList.remove('pointer-events-none', 'opacity-0');
    elements.cartOverlay.classList.add('pointer-events-auto', 'opacity-100');

    elements.cartPanel.classList.remove('pointer-events-none', 'translate-x-full', 'opacity-0');
    elements.cartPanel.classList.add('pointer-events-auto', 'translate-x-0', 'opacity-100');

    elements.cartPanel.setAttribute('aria-hidden', 'false');
    elements.cartToggleBtn.setAttribute('aria-expanded', 'true');
    return;
  }

  elements.cartOverlay.classList.remove('pointer-events-auto', 'opacity-100');
  elements.cartOverlay.classList.add('pointer-events-none', 'opacity-0');

  elements.cartPanel.classList.remove('pointer-events-auto', 'translate-x-0', 'opacity-100');
  elements.cartPanel.classList.add('pointer-events-none', 'translate-x-full', 'opacity-0');

  elements.cartPanel.setAttribute('aria-hidden', 'true');
  elements.cartToggleBtn.setAttribute('aria-expanded', 'false');
}

function animateBadge() {
  elements.cartCountBadge.classList.remove('animate-badge-pop');
  void elements.cartCountBadge.offsetWidth;
  elements.cartCountBadge.classList.add('animate-badge-pop');
}

let previousItemsCount = 0;

function render(shouldAnimateBadge) {
  renderProducts();
  renderCartItems();
  renderTotals();
  renderCartVisibility();

  if (!shouldAnimateBadge) {
    return;
  }

  const currentItemsCount = getTotals().itemsCount;
  if (currentItemsCount !== previousItemsCount) {
    animateBadge();
    previousItemsCount = currentItemsCount;
  }
}

elements.productsGrid.addEventListener('click', (event) => {
  const button = event.target.closest("[data-action='add']");
  if (!button) {
    return;
  }

  addToCart(button.dataset.productId);
});

elements.cartToggleBtn.addEventListener('click', toggleCart);
elements.closeCartBtn.addEventListener('click', closeCart);
elements.cartOverlay.addEventListener('click', closeCart);
elements.clearCartBtn.addEventListener('click', clearCart);

document.addEventListener('keydown', (event) => {
  if (event.key === 'Escape') {
    closeCart();
  }
});

render(false);
