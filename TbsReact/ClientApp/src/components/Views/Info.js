import React from "react";
import { Grid, Typography } from "@mui/material";
import ContentBox from "../ContentBox";

const Info = () => {
	return (
		<>
			<ContentBox>
				<Grid spacing={2} container>
					<Grid item xs={12}>
						<ContentBox>
							<Typography
								variant="h4"
								noWrap
								component="div"
								sx={{
									mr: 2,
									display: { xs: "none", md: "flex" },
								}}
							>
								TBS - React version
							</Typography>
							Build:
							{process.env.REACT_APP_TBS_VERSION}
						</ContentBox>
					</Grid>
					<Grid item xs={6}>
						<ContentBox>
							Join our Discord Server
							<br />
							<a
								href="https://discord.gg/mBa4f2K"
								target="_blank"
								rel="noreferrer"
							>
								<img
									alt="TravianBotSharp Discord server"
									src="https://discordapp.com/api/guilds/740829801446637601/widget.png?style=banner2"
								></img>
							</a>
						</ContentBox>
					</Grid>

					<Grid item xs={6}>
						<ContentBox>
							<div>
								If you would like to make a donation, here it is{" "}
								<br />
								<a
									href="https://ko-fi.com/T6T3648VG"
									target="_blank"
									rel="noreferrer"
								>
									<img
										height="36"
										style={{
											border: "0px",
											height: "36px",
										}}
										src="https://cdn.ko-fi.com/cdn/kofi3.png?v=3"
										border="0"
										alt="Buy Me a Coffee at ko-fi.com"
									/>
								</a>
							</div>
							<div>
								In case you are Vietnamese, đây là link cho Momo
								._. <br />
								<a
									href="https://nhantien.momo.vn/8C4j4DfmHO3"
									target="_blank"
									rel="noreferrer"
								>
									<img
										height="36"
										style={{
											border: "0px",
											height: "36px",
										}}
										src="https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png"
										border="0"
										alt="Buy Me a Coffee at ko-fi.com"
									/>
								</a>
							</div>
						</ContentBox>
					</Grid>
				</Grid>
			</ContentBox>
		</>
	);
};

export default Info;
