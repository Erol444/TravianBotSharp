import axios from "axios";

const setHeaderToken = (token) => {
	axios.defaults.headers.common.token = `${token}`;
};

export { setHeaderToken };
