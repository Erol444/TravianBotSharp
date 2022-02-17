import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Grid, Typography, TextField, Button } from "@mui/material";
import { yupResolver } from "@hookform/resolvers/yup";
import { toast } from "react-toastify";
import { getSetting, setSetting, CHROME_SETTING } from "../../../api/api";
import { useSelector } from "react-redux";
import MUISwitch from "../../ref/MUISwitch";

import ContentBox from "../../ContentBox";
import ChromeSchema from "../../../yup/Settings/ChomreSchema";

const ChromeSettings = () => {
	const {
		register,
		handleSubmit,
		formState: { errors },
		setValue,
		control,
	} = useForm({
		resolver: yupResolver(ChromeSchema),
	});
	const account = useSelector((state) => state.account.info.id);
	const onSubmit = (data) => {
		setSetting(account, CHROME_SETTING, data).then((result) => {
			if (result === true) {
				toast.success("Chrome settings saved!", {
					position: toast.POSITION.TOP_RIGHT,
				});
			} else {
				toast.warning("Chrome settings not saved! Try again later", {
					position: toast.POSITION.TOP_RIGHT,
				});
			}
		});
	};

	useEffect(() => {
		if (account !== -1) {
			getSetting(account, CHROME_SETTING).then((data) => {
				const { disableImages, click, autoClose } = data;
				setValue("click.min", click.min);
				setValue("click.max", click.max);
				setValue("disableImages", disableImages);
				setValue("autoClose", autoClose);
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
							<MUISwitch
								name="disableImages"
								control={control}
								label="Disable images"
							/>
						</Grid>
						<Grid item xs={12}>
							<MUISwitch
								name="autoClose"
								control={control}
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
