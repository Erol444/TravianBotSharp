import PropTypes from "prop-types";
import React from "react";
import { Button, TextField } from "@mui/material";
import { useForm } from "react-hook-form";
const TokenInput = ({ setToken }) => {
	const { register, handleSubmit } = useForm();

	const onSubmit = (data) => {
		setToken(data.token);
	};
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
