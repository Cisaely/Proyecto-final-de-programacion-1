// =============================================
// NexBusiness - Utilidades compartidas
// =============================================

const API = '/api';

// ── Auth helpers ──────────────────────────────
function getToken() { return localStorage.getItem('nb_token'); }
function getTipo()  { return localStorage.getItem('nb_tipo'); }
function getId()    { return localStorage.getItem('nb_id'); }
function getNombre(){ return localStorage.getItem('nb_nombre'); }

function setSession(data) {
  localStorage.setItem('nb_token',  data.token);
  localStorage.setItem('nb_tipo',   data.tipo);
  localStorage.setItem('nb_id',     data.id);
  localStorage.setItem('nb_nombre', data.nombre ?? data.nombreEmpresa ?? 'Usuario');
}

function clearSession() {
  ['nb_token','nb_tipo','nb_id','nb_nombre'].forEach(k => localStorage.removeItem(k));
}

function requireAuth(expectedTipo = null) {
  const token = getToken();
  if (!token) { window.location.href = '/login.html'; return false; }
  if (expectedTipo && getTipo() !== expectedTipo) {
    window.location.href = getTipo() === 'empresa'
      ? '/empresa/index.html' : '/buscador/index.html';
    return false;
  }
  return true;
}

function redirectIfLoggedIn() {
  if (getToken()) {
    window.location.href = getTipo() === 'empresa'
      ? '/empresa/index.html' : '/buscador/index.html';
  }
}

// ── Fetch con auth ────────────────────────────
async function apiFetch(path, options = {}) {
  const token = getToken();
  const res = await fetch(API + path, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {})
    }
  });
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw new Error(data.mensaje || data.title || 'Error en la petición');
  return data;
}

// ── Toast notifications ───────────────────────
function showToast(msg, type = 'default') {
  const existing = document.getElementById('toast-global');
  if (existing) existing.remove();
  const t = document.createElement('div');
  t.id = 'toast-global';
  t.className = `toast ${type}`;
  t.innerHTML = `
    <svg width="20" height="20" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
      ${type === 'success' ? '<path d="M20 6L9 17l-5-5"/>'
        : type === 'error' ? '<circle cx="12" cy="12" r="10"/><path d="M15 9l-6 6M9 9l6 6"/>'
        : '<circle cx="12" cy="12" r="10"/><path d="M12 8v4m0 4h.01"/>'}
    </svg>
    <span>${msg}</span>`;
  document.body.appendChild(t);
  setTimeout(() => t.remove(), 3500);
}

// ── Modal ─────────────────────────────────────
function openModal(id)  { document.getElementById(id).style.display = 'flex'; }
function closeModal(id) { document.getElementById(id).style.display = 'none'; }

// Bloqueamos el cierre al hacer click afuera para no perder progreso
// document.addEventListener('click', e => {
//   if (e.target.classList.contains('modal-overlay')) {
//     e.target.style.display = 'none';
//   }
// });

// ── Sidebar móvil ─────────────────────────────
function initSidebar() {
  const sidebar  = document.getElementById('sidebar');
  const overlay  = document.getElementById('sidebar-overlay');
  const btnOpen  = document.getElementById('btn-open-sidebar');
  const btnClose = document.getElementById('btn-close-sidebar');

  if (!sidebar) return;

  function open()  { sidebar.classList.add('open'); overlay.classList.add('visible'); }
  function close() { sidebar.classList.remove('open'); overlay.classList.remove('visible'); }

  btnOpen?.addEventListener('click', open);
  btnClose?.addEventListener('click', close);
  overlay?.addEventListener('click', close);
}

// ── Logout ────────────────────────────────────
function logout() {
  clearSession();
  window.location.href = '/index.html';
}

// ── Poblar nombre en sidebar ──────────────────
function populateSidebarUser() {
  const nameEl = document.getElementById('sidebar-name');
  const roleEl = document.getElementById('sidebar-role');
  if (nameEl) nameEl.textContent = getNombre() || 'Usuario';
  if (roleEl) roleEl.textContent = getTipo() === 'empresa' ? 'Empresa' : 'Candidato';
  const avatarEl = document.getElementById('sidebar-avatar');
  if (avatarEl) avatarEl.textContent = (getNombre() || 'U')[0].toUpperCase();
}

// ── Formatear fecha ───────────────────────────
function fmtFecha(iso) {
  if (!iso) return '—';
  return new Date(iso).toLocaleDateString('es-ES', { day:'2-digit', month:'short', year:'numeric' });
}

// ── Badges de estado ──────────────────────────
function estadoBadge(estado) {
  const map = { Pendiente: 'warning', Aceptado: 'success', Rechazado: 'danger',
                Activa: 'success', Inactiva: 'gray' };
  return `<span class="badge badge-${map[estado] || 'gray'}">${estado}</span>`;
}

// Init al cargar
document.addEventListener('DOMContentLoaded', () => {
  initSidebar();
  populateSidebarUser();
});
