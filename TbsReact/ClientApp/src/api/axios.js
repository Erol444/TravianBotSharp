import axios from "axios";

const setHeaderToken = (token) => {
	window.localStorage.setItem("token", token);
	axios.defaults.headers.common.token = `${token}`;
};

const getHeaderToken = () => {
	const token = window.localStorage.getItem("token");
	axios.defaults.headers.common.token = `${token}`;
};

export { setHeaderToken, getHeaderToken };
