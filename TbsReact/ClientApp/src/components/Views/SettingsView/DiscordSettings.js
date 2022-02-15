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

import ContentBox from "../../ContentBox";

const DiscordSettings = () => {
	const { register, handleSubmit, getValues } = useForm();
	const onSubmit = (data) => console.log(data);
	const onChecking = () => console.log(getValues("url"));
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
