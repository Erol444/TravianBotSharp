import PropTypes from "prop-types";
import React, { useEffect } from "react";
import { toast } from "react-toastify";

import { Button, TextField } from "@mui/material";
import { useForm } from "react-hook-form";
import axios from "axios";
import { setHeaderToken, getHeaderToken } from "../api/axios";
const TokenInput = ({ setToken }) => {
	const { register, handleSubmit } = useForm();

	const onSubmit = async (token) => {
		setHeaderToken(token.token);
		try {
			await axios.get("/api/checkToken");
			setToken(true);
		} catch (err) {
			if (err.response.status === 401) {
				toast.error("Invalid token");
			}
		}
	};
	useEffect(() => {
		getHeaderToken();

		axios
			.get("/api/checkToken")
			.then(() => {
				setToken(true);
			})
			.catch((err) => {
				if (err.response.status === 401) {
					toast.warn("Input token from TBS console to continue");
				}
			});
	}, [setToken]);
	return (
		<>
			<form onSubmit={handleSubmit(onSubmit)}>
				<TextField
					fullWidth
					label="Token"
					variant="outlined"
					{...register("token")}
				/>
				<Button
					type="submit"
					variant="contained"
					value="submit"
					color="success"
				>
					Save
				</Button>
			</form>
		</>
	);
};

TokenInput.propTypes = {
	setToken: PropTypes.func.isRequired,
};

export default TokenInput;
