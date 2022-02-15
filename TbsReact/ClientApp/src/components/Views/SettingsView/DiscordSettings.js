import {
	Button,
	FormControlLabel,
	Grid,
	Switch,
	TextField,
	Typography,
} from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { checkUrlDiscordWebhook } from "../../../api/Discord";
import ContentBox from "../../ContentBox";

const DiscordSettings = () => {
	const {
		register,
		handleSubmit,
		getValues,
		formState: { errors },
		setError,
	} = useForm();

	const onSubmit = async (data) => {
		const isUrlValid = await checkUrl(getValues("url"), false);
		if (!isUrlValid) return;
		console.log(data);
	};
	const onChecking = async () => {
		const url = getValues("url");
		await checkUrl(url, true);
	};

	const checkUrl = async (url, show) => {
		if (url === "") return;
		const data = await checkUrlDiscordWebhook(url);
		switch (data) {
			case 200:
				if (show) {
					toast.success("Webhook url is valid", {
						position: toast.POSITION.TOP_RIGHT,
					});
				}
				return true;
			case 401:
			case 404:
				toast.error("Webhook url is invaild", {
					position: toast.POSITION.TOP_RIGHT,
				});
				setError("url", { message: "Reinput webhook url" });

				return false;
		}
	};

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Discord Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={12}>
							<FormControlLabel
								control={<Switch {...register("active")} />}
								label="Active"
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={<Switch {...register("online")} />}
								label="Send message when account online"
							/>
						</Grid>
						<Grid item xs={8}>
							<TextField
								fullWidth
								label="URL webhook"
								variant="outlined"
								error={!!errors.url}
								helperText={errors.url?.message}
								{...register("url")}
							/>
						</Grid>
						<Grid item xs={4}>
							<Button variant="contained" onClick={onChecking}>
								Check
							</Button>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Save
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};
export default DiscordSettings;
