const PRODUCTS = [
  {
    id: "p-101",
    name: "Наушники Orbit Pro",
    category: "Аудио",
    price: 7490,
    description: "Шумоподавление, до 36 часов автономной работы.",
  },
  {
    id: "p-102",
    name: "Клавиатура Flow TKL",
    category: "Периферия",
    price: 5690,
    description: "Механические свитчи, горячая замена, RGB-подсветка.",
  },
  {
    id: "p-103",
    name: "Монитор Nova 27",
    category: "Мониторы",
    price: 21990,
    description: "27 дюймов, IPS, 165 Гц, поддержка HDR.",
  },
  {
    id: "p-104",
    name: "Мышь Pulse X",
    category: "Периферия",
    price: 3290,
    description: "Легкий корпус, 26000 DPI, беспроводной режим.",
  },
  {
    id: "p-105",
    name: "SSD FlashCore 1TB",
    category: "Накопители",
    price: 8890,
    description: "PCIe 4.0 NVMe, скорость чтения до 7000 МБ/с.",
  },
  {
    id: "p-106",
    name: "Веб-камера Stream 2K",
    category: "Стрим",
    price: 4590,
    description: "2K-разрешение, автофокус, два микрофона.",
  },
];

const productById = new Map(PRODUCTS.map((product) => [product.id, product]));
const moneyFormatter = new Intl.NumberFormat("ru-RU", {
  style: "currency",
  currency: "RUB",
  maximumFractionDigits: 0,
});

const elements = {
  productsGrid: document.getElementById("products-grid"),
  catalogSummary: document.getElementById("catalog-summary"),
  cartToggleBtn: document.getElementById("cart-toggle-btn"),
  closeCartBtn: document.getElementById("close-cart-btn"),
  cartCountBadge: document.getElementById("cart-count-badge"),
  cartOverlay: document.getElementById("cart-overlay"),
  cartPanel: document.getElementById("cart-panel"),
  cartItemsList: document.getElementById("cart-items-list"),
  emptyCartState: document.getElementById("empty-cart-state"),
  cartSubtitle: document.getElementById("cart-subtitle"),
  subtotalCount: document.getElementById("subtotal-count"),
  subtotalValue: document.getElementById("subtotal-value"),
  clearCartBtn: document.getElementById("clear-cart-btn"),
};

function createStore(initialState) {
  let state = initialState;
  const listeners = new Set();

  return {
    getState() {
      return state;
    },
    subscribe(listener) {
      listeners.add(listener);
      return () => listeners.delete(listener);
    },
    setState(updater) {
      const nextState = updater(state);
      if (nextState === state) {
        return;
      }

      state = nextState;
      listeners.forEach((listener) => listener(state));
    },
  };
}

const store = createStore({
  products: PRODUCTS,
  cart: {},
  isCartOpen: false,
  badgeBumpToken: 0,
});

const actions = {
  toggleCart() {
    store.setState((prev) => ({ ...prev, isCartOpen: !prev.isCartOpen }));
  },
  closeCart() {
    store.setState((prev) => {
      if (!prev.isCartOpen) {
        return prev;
      }

      return { ...prev, isCartOpen: false };
    });
  },
  addToCart(productId) {
    if (!productById.has(productId)) {
      return;
    }

    store.setState((prev) => {
      const nextQuantity = (prev.cart[productId] ?? 0) + 1;

      return {
        ...prev,
        cart: {
          ...prev.cart,
          [productId]: nextQuantity,
        },
        badgeBumpToken: prev.badgeBumpToken + 1,
      };
    });
  },
  increaseItem(productId) {
    actions.addToCart(productId);
  },
  decreaseItem(productId) {
    store.setState((prev) => {
      const currentQuantity = prev.cart[productId] ?? 0;
      if (currentQuantity === 0) {
        return prev;
      }

      if (currentQuantity === 1) {
        const { [productId]: removed, ...restCart } = prev.cart;
        void removed;

        return {
          ...prev,
          cart: restCart,
          badgeBumpToken: prev.badgeBumpToken + 1,
        };
      }

      return {
        ...prev,
        cart: {
          ...prev.cart,
          [productId]: currentQuantity - 1,
        },
        badgeBumpToken: prev.badgeBumpToken + 1,
      };
    });
  },
  removeItem(productId) {
    store.setState((prev) => {
      if (!prev.cart[productId]) {
        return prev;
      }

      const { [productId]: removed, ...restCart } = prev.cart;
      void removed;

      return {
        ...prev,
        cart: restCart,
        badgeBumpToken: prev.badgeBumpToken + 1,
      };
    });
  },
  clearCart() {
    store.setState((prev) => {
      if (Object.keys(prev.cart).length === 0) {
        return prev;
      }

      return {
        ...prev,
        cart: {},
        badgeBumpToken: prev.badgeBumpToken + 1,
      };
    });
  },
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

function getCartTotals(state) {
  return Object.entries(state.cart).reduce(
    (totals, [productId, quantity]) => {
      const product = productById.get(productId);
      if (!product) {
        return totals;
      }

      return {
        itemsCount: totals.itemsCount + quantity,
        totalPrice: totals.totalPrice + product.price * quantity,
      };
    },
    { itemsCount: 0, totalPrice: 0 },
  );
}

function getCartItems(state) {
  return Object.entries(state.cart)
    .map(([productId, quantity]) => {
      const product = productById.get(productId);
      if (!product) {
        return null;
      }

      return {
        ...product,
        quantity,
        lineTotal: product.price * quantity,
      };
    })
    .filter(Boolean);
}

function renderProducts(state) {
  elements.productsGrid.innerHTML = state.products
    .map((product) => {
      const quantity = state.cart[product.id] ?? 0;
      const quantityBadge =
        quantity > 0
          ? `<span class="rounded-full border border-emerald-400/40 bg-emerald-500/20 px-2 py-1 text-xs font-bold text-emerald-200">В корзине: ${quantity}</span>`
          : '<span class="text-xs text-slate-400">Еще не добавлен</span>';

      return `
        <li class="rounded-2xl border border-slate-800 bg-slate-900/70 p-4 transition hover:-translate-y-0.5 hover:border-cyan-400/40">
          <div class="flex items-start justify-between gap-3">
            <p class="rounded-full border border-slate-700 bg-slate-800 px-2.5 py-1 text-xs font-semibold text-slate-300">${product.category}</p>
            ${quantityBadge}
          </div>

          <h3 class="mt-4 text-lg font-bold">${product.name}</h3>
          <p class="mt-2 text-sm text-slate-300">${product.description}</p>

          <div class="mt-5 flex items-center justify-between gap-3">
            <p class="text-base font-extrabold text-cyan-300">${formatPrice(product.price)}</p>
            <button
              type="button"
              data-product-id="${product.id}"
              data-action="add"
              class="rounded-full bg-cyan-400 px-4 py-2 text-xs font-extrabold uppercase tracking-[0.08em] text-slate-950 transition hover:bg-cyan-300"
            >
              В корзину
            </button>
          </div>
        </li>
      `;
    })
    .join("");
}

function renderCartItems(state) {
  const cartItems = getCartItems(state);

  if (cartItems.length === 0) {
    elements.emptyCartState.classList.remove("hidden");
    elements.cartItemsList.classList.add("hidden");
    elements.cartItemsList.innerHTML = "";
    elements.clearCartBtn.disabled = true;
    return;
  }

  elements.emptyCartState.classList.add("hidden");
  elements.cartItemsList.classList.remove("hidden");
  elements.clearCartBtn.disabled = false;

  elements.cartItemsList.innerHTML = cartItems
    .map((item) => {
      return `
        <li class="rounded-2xl border border-slate-800 bg-slate-800/70 p-4">
          <div class="flex items-start justify-between gap-3">
            <div>
              <h3 class="text-sm font-bold text-slate-100">${item.name}</h3>
              <p class="mt-1 text-xs text-slate-300">${formatPrice(item.price)} за шт.</p>
            </div>
            <button
              type="button"
              data-cart-action="remove"
              data-product-id="${item.id}"
              class="rounded-full border border-slate-700 px-2.5 py-1 text-xs font-semibold text-slate-300 transition hover:border-red-400 hover:text-red-200"
            >
              Удалить
            </button>
          </div>

          <div class="mt-3 flex items-center justify-between gap-3">
            <div class="inline-flex items-center gap-1 rounded-full border border-slate-700 bg-slate-900 p-1">
              <button
                type="button"
                data-cart-action="decrease"
                data-product-id="${item.id}"
                class="h-7 w-7 rounded-full text-sm font-bold text-slate-200 transition hover:bg-slate-700"
                aria-label="Уменьшить количество"
              >
                -
              </button>
              <span class="min-w-8 text-center text-sm font-bold text-slate-100">${item.quantity}</span>
              <button
                type="button"
                data-cart-action="increase"
                data-product-id="${item.id}"
                class="h-7 w-7 rounded-full text-sm font-bold text-slate-200 transition hover:bg-slate-700"
                aria-label="Увеличить количество"
              >
                +
              </button>
            </div>

            <p class="text-sm font-extrabold text-cyan-300">${formatPrice(item.lineTotal)}</p>
          </div>
        </li>
      `;
    })
    .join("");
}

function renderCartTotals(state) {
  const totals = getCartTotals(state);

  elements.cartCountBadge.textContent = String(totals.itemsCount);
  elements.catalogSummary.textContent = `В корзине: ${totals.itemsCount} шт.`;
  elements.cartSubtitle.textContent = pluralizeItems(totals.itemsCount);
  elements.subtotalCount.textContent = pluralizeItems(totals.itemsCount);
  elements.subtotalValue.textContent = formatPrice(totals.totalPrice);
}

function renderCartVisibility(state) {
  if (state.isCartOpen) {
    elements.cartOverlay.classList.remove("pointer-events-none", "opacity-0");
    elements.cartOverlay.classList.add("pointer-events-auto", "opacity-100");

    elements.cartPanel.classList.remove(
      "pointer-events-none",
      "translate-x-full",
      "opacity-0",
    );
    elements.cartPanel.classList.add("pointer-events-auto", "translate-x-0", "opacity-100");

    elements.cartPanel.setAttribute("aria-hidden", "false");
    elements.cartToggleBtn.setAttribute("aria-expanded", "true");
    return;
  }

  elements.cartOverlay.classList.remove("pointer-events-auto", "opacity-100");
  elements.cartOverlay.classList.add("pointer-events-none", "opacity-0");

  elements.cartPanel.classList.remove("pointer-events-auto", "translate-x-0", "opacity-100");
  elements.cartPanel.classList.add("pointer-events-none", "translate-x-full", "opacity-0");

  elements.cartPanel.setAttribute("aria-hidden", "true");
  elements.cartToggleBtn.setAttribute("aria-expanded", "false");
}

function animateBadge() {
  elements.cartCountBadge.classList.remove("animate-badge-pop");
  void elements.cartCountBadge.offsetWidth;
  elements.cartCountBadge.classList.add("animate-badge-pop");
}

let previousBadgeToken = store.getState().badgeBumpToken;

function render(state) {
  renderProducts(state);
  renderCartItems(state);
  renderCartTotals(state);
  renderCartVisibility(state);

  if (state.badgeBumpToken !== previousBadgeToken) {
    animateBadge();
    previousBadgeToken = state.badgeBumpToken;
  }
}

store.subscribe(render);

elements.productsGrid.addEventListener("click", (event) => {
  const button = event.target.closest("[data-action='add']");
  if (!button) {
    return;
  }

  actions.addToCart(button.dataset.productId);
});

elements.cartItemsList.addEventListener("click", (event) => {
  const button = event.target.closest("[data-cart-action]");
  if (!button) {
    return;
  }

  const productId = button.dataset.productId;
  const actionName = button.dataset.cartAction;

  if (actionName === "increase") {
    actions.increaseItem(productId);
    return;
  }

  if (actionName === "decrease") {
    actions.decreaseItem(productId);
    return;
  }

  if (actionName === "remove") {
    actions.removeItem(productId);
  }
});

elements.cartToggleBtn.addEventListener("click", () => {
  actions.toggleCart();
});

elements.closeCartBtn.addEventListener("click", () => {
  actions.closeCart();
});

elements.cartOverlay.addEventListener("click", () => {
  actions.closeCart();
});

elements.clearCartBtn.addEventListener("click", () => {
  actions.clearCart();
});

document.addEventListener("keydown", (event) => {
  if (event.key === "Escape") {
    actions.closeCart();
  }
});

render(store.getState());
