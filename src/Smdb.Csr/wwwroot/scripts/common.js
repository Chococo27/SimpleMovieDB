export const API_BASE = 'http://localhost:8080/api/v1'; //Direccion Base
export const $ = (sel, el = document) => el.querySelector(sel); //Short hand
export const $$ = (sel, el = document) => Array.from(el.querySelectorAll(sel)); //Short hand
export const getQueryParam = (k) => new URLSearchParams(location.search).get(k);

//Recibe y provee Json
function jsonHeaders() {
	return { 'Content-Type': 'application/json', 'Accept': 'application/json' };
}

//Sele pasan los paths y opciones deceadas
//(de faltarle el segunfo parametro automaticamente pone un objeto vacio)
//De el path comensar con http ese sera el URL
//De no el URL es API_BASE + path
export async function apiFetch(path, opts = {}) {
	const url = path.startsWith('http') ? path : `${API_BASE}${path}`;
	const init = {
		...opts,
		headers: { ...(opts.headers || {}), ...jsonHeaders() }
	};
	const res = await fetch(url, init);
	const text = await res.text();
	let payload = null;
	try { payload = text ? JSON.parse(text) : null; } catch { payload = text; }

	if (!res.ok) {
		const msg = (payload && (payload.message || payload.error)) ||
			`${res.status} ${res.statusText}`;
		const err = new Error(msg);
		err.status = res.status;
		err.payload = payload;
		throw err;
	}
	return payload;
}

export function renderStatus(el, type, message) {
	if (!el) return;
	el.className = `status ${type}`;
	el.textContent = message;
}

export function clearChildren(el) {
	//while (el.firstChild) el.removeChild(el.firstChild); 
	el.replaceChildren();
}

export function captureMovieForm(form) {
	const title = form.title.value.trim();
	const year = Number(form.year.value);
	const description = form.description.value.trim();
	return { title, year, description };
} 