import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import {
	Grid,
	Typography,
	TextField,
	Button,
	Switch,
	FormControlLabel,
} from "@mui/material";
import { yupResolver } from "@hookform/resolvers/yup";
import { toast } from "react-toastify";

import { getChromeSetting, setChromeSetting } from "../../../api/Setting";
import { useSelector } from "react-redux";

import ContentBox from "../../ContentBox";
import ChromeSchema from "../../../yup/Settings/ChomreSchema.js";

const ChromeSettings = () => {
	const {
		register,
		handleSubmit,
		formState: { errors },
		setValue,
	} = useForm({
		resolver: yupResolver(ChromeSchema),
	});
	const account = useSelector((state) => state.account.info.id);
	const onSubmit = async (data) => {
		await setChromeSetting(account, data);
		toast.success("Chrome settings saved !", {
			position: toast.POSITION.TOP_RIGHT,
		});
	};

	useEffect(() => {
		if (account !== -1) {
			getChromeSetting(account).then((data) => {
				const { disableImages, click, autoClose } = data;
				setValue("click.min", click.min);
				setValue("click.max", click.max);
				setValue("disable_image", disableImages);
				setValue("close_chrome", autoClose);
			});
		}
	}, [account, setValue]);

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Chrome Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Click delay:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.click?.min}
								helperText={errors.click?.min?.message}
								{...register("click.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.click?.max}
								helperText={errors.click?.max?.message}
								{...register("click.max")}
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={
									<Switch {...register("disable_image")} />
								}
								label="Disable images"
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={
									<Switch {...register("close_chrome")} />
								}
								label="Close if no task in 5 minutes"
							/>
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

export default ChromeSettings;
