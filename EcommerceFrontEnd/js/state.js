const storageKey = "phone-orders-session";

function readSession() {
  try { return JSON.parse(localStorage.getItem(storageKey)); } catch { return null; }
}

export const state = {
  session: readSession(),
  products: [],
  orders: [],
  cart: [],
  productPage: 0,
  expandedOrderId: null,
  variantIndexes: {},
  pendingVariantId: null
};

export function saveSession(session) {
  state.session = session;
  localStorage.setItem(storageKey, JSON.stringify(session));
}

export function clearSession() {
  state.session = null;
  localStorage.removeItem(storageKey);
}
